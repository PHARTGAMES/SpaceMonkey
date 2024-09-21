-- This Source Code Form is subject to the terms of the bCDDL, v. 1.1.
-- If a copy of the bCDDL was not distributed with this
-- file, You can obtain one at http://beamng.com/bCDDL-1.1.txt

local ffi = require("ffi")

local M = {}
local protocols = {}
local definedStructs = {}
local physicsStepUsed = false

-- smoother variables
local rollVelSmoothedPrev, pitchVelSmoothedPrev, yawVelSmoothedPrev
local velXSmoother, velYSmoother, velZSmoother
local accXSmoother, accYSmoother, accZSmoother
local rollPosSmoother, pitchPosSmoother, yawPosSmoother
local rollVelSmoother, pitchVelSmoother, yawVelSmoother
local rollAccSmoother, pitchAccSmoother, yawAccSmoother

local dtSimAccumulator = 0

local function updateProtocol(protocol, dtSim)
  dtSimAccumulator = dtSimAccumulator + dtSim
  local now = protocol.updateTimer:stop() * 0.001 -- this has to be wall clock time (not sim time), the user wants certain Hz of update rate, regardless of slowmotion mode, etc
  local elapsed = now - protocol.nextUpdate
  local pendingUpdates = math.ceil(elapsed / protocol.updatePeriod)
  protocol.nextUpdate = protocol.nextUpdate + protocol.updatePeriod * pendingUpdates
  if pendingUpdates > 0 then
    ffi.fill(protocol.packet, protocol.packetSize) -- memset to zero
    protocol.module.fillStruct(protocol.packet, dtSimAccumulator)--dtSim)
	dtSimAccumulator = 0
    local data = ffi.string(protocol.packet, protocol.packetSize)
    protocol.udpSocket:sendto(data, protocol.ip, protocol.port)
  end
end

local function updateSmoothers(dtSim)
  M.posX, M.posY, M.posZ = obj:getPositionXYZ()

  local velX, velY, velZ = obj:getVelocityXYZ()
  M.velXSmoothed = velXSmoother:get(velX, dtSim)
  M.velYSmoothed = velYSmoother:get(velY, dtSim)
  M.velZSmoothed = velZSmoother:get(velZ, dtSim)

  local ffisensors = sensors.ffiSensors
  M.accXSmoothed = accXSmoother:get(-ffisensors.sensorX, dtSim)
  M.accYSmoothed = accYSmoother:get(-ffisensors.sensorY, dtSim)
  M.accZSmoothed = accZSmoother:get(-ffisensors.sensorZ, dtSim)

  local up = obj:getDirectionVectorUp()
  M.upX = up.x
  M.upY = up.y
  M.upZ = up.z

  local roll, pitch, yaw = obj:getRollPitchYaw()
  M.rollPosSmoothed = rollPosSmoother:get(-roll, dtSim) --negated angle here, seems like that is the "standard" for motion sims here
  M.pitchPosSmoothed = pitchPosSmoother:get(pitch, dtSim)
  M.yawPosSmoothed = yawPosSmoother:get(-yaw, dtSim) --negated angle here, seems like that is the "standard" for motion sims here

  local rollVel, pitchVel, yawVel = obj:getRollPitchYawAngularVelocity()
  M.rollVelSmoothed = rollVelSmoother:get(rollVel, dtSim)
  M.pitchVelSmoothed = pitchVelSmoother:get(pitchVel, dtSim)
  M.yawVelSmoothed = yawVelSmoother:get(yawVel, dtSim)

  local dtCoef = 1 / dtSim
  M.rollAccSmoothed = rollAccSmoother:get((M.rollVelSmoothed - rollVelSmoothedPrev) * dtCoef, dtSim)
  M.pitchAccSmoothed = pitchAccSmoother:get((M.pitchVelSmoothed - pitchVelSmoothedPrev) * dtCoef, dtSim)
  M.yawAccSmoothed = yawAccSmoother:get((M.yawVelSmoothed - yawVelSmoothedPrev) * dtCoef, dtSim)

  rollVelSmoothedPrev = M.rollVelSmoothed
  pitchVelSmoothedPrev = M.pitchVelSmoothed
  yawVelSmoothedPrev = M.yawVelSmoothed
end

local function updatePhysics(dtSim)
  if not playerInfo.firstPlayerSeated then return end
  if physicsStepUsed then
    updateSmoothers(dtSim)
  end
  for _,protocol in ipairs(protocols) do
    if protocol.physicsStepUsed then
      updateProtocol(protocol, dtSim)
    end
  end
end

local function updateGFX(dtSim)
  if not playerInfo.firstPlayerSeated then return end
  if not physicsStepUsed then
    updateSmoothers(dtSim)
  end
  --guihooks.graph({"Acc Roll", M.rollAcc, 1, "", true}, {"Acc Pitch", M.pitchAcc, 1, "", true}, {"Acc Yaw", M.yawAccSmoothed, 1, "", true})
  for _,protocol in ipairs(protocols) do
    if not protocol.physicsStepUsed then
      updateProtocol(protocol, dtSim)
    end
  end
end

local function smoothersReset()
  if not next(protocols) then return end
  rollVelSmoothedPrev = 0
  pitchVelSmoothedPrev = 0
  yawVelSmoothedPrev = 0

  velXSmoother:reset()
  velYSmoother:reset()
  velZSmoother:reset()

  accXSmoother:reset()
  accYSmoother:reset()
  accZSmoother:reset()

  rollPosSmoother:reset()
  pitchPosSmoother:reset()
  yawPosSmoother:reset()

  rollVelSmoother:reset()
  pitchVelSmoother:reset()
  yawVelSmoother:reset()

  rollAccSmoother:reset()
  pitchAccSmoother:reset()
  yawAccSmoother:reset()
end

local function reset()
  smoothersReset()
  for _,protocol in ipairs(protocols) do
    if type(protocol.module.reset) == "function" then
      protocol.module.reset()
    end
  end
end

local function destroyProtocol(protocol)
  if protocol.udpSocket then
    protocol.udpSocket:close()
    protocol.udpSocket = nil
  end
  local idx = tableFindKey(protocols, protocol)
  if not idx then
    log("E", "", "Unable to destroy protocol '"..dumps(protocol.moduleName).."', not found in protocols table")
    return
  end
  table.remove(protocols, idx)
end

-- the config looks similar to: { enabled=true, name="motionSim", ...}
local function initProtocolFromConfig(name)
  local protocol = {}
  protocol.name = name
  protocol.structName = "protocol_"..protocol.name.."_t"
  protocol.moduleName = "protocols/"..protocol.name
  protocol.module = require(protocol.moduleName)
  if not protocol.module then
    log("W", "", "Unable to load protocol, require() failed for module: "..dumps(protocol.moduleName))
    return
  end
  if type(protocol.module.init) == "function" then
    protocol.module.init()
  end
  protocol.ip = protocol.module.getAddress()
  protocol.port = protocol.module.getPort()
  protocol.physicsStepUsed = protocol.module.isPhysicsStepUsed()
  protocol.updateRate = protocol.module.getMaxUpdateRate()
  protocol.updatePeriod = 1 / protocol.updateRate
  protocol.updateTimer = HighPerfTimer()
  protocol.updateTimer:reset()
  protocol.nextUpdate = protocol.updateTimer:stop()
  protocol.udpSocket = socket.udp()
  if not definedStructs[protocol.name] then
    ffi.cdef("typedef struct "..protocol.structName.." {\n"..protocol.module.getStructDefinition().."} "..protocol.structName..";")
    definedStructs[protocol.name] = true
  end
  protocol.packet = ffi.new(protocol.structName)
  protocol.packetSize = ffi.sizeof(protocol.packet)
  return protocol
end

local function smoothersInit()
  rollVelSmoothedPrev = 0
  pitchVelSmoothedPrev = 0
  yawVelSmoothedPrev = 0

  velXSmoother     = newExponentialSmoothingT(settings.getValue("protocols_smoothing_velX"    ))
  velYSmoother     = newExponentialSmoothingT(settings.getValue("protocols_smoothing_velY"    ))
  velZSmoother     = newExponentialSmoothingT(settings.getValue("protocols_smoothing_velZ"    ))

  accXSmoother     = newExponentialSmoothingT(settings.getValue("protocols_smoothing_accX"    ))
  accYSmoother     = newExponentialSmoothingT(settings.getValue("protocols_smoothing_accY"    ))
  accZSmoother     = newExponentialSmoothingT(settings.getValue("protocols_smoothing_accZ"    ))

  rollPosSmoother  = newExponentialSmoothingT(settings.getValue("protocols_smoothing_rollPos" ))
  pitchPosSmoother = newExponentialSmoothingT(settings.getValue("protocols_smoothing_pitchPos"))
  yawPosSmoother   = newExponentialSmoothingT(settings.getValue("protocols_smoothing_yawPos"  ))

  rollVelSmoother  = newExponentialSmoothingT(settings.getValue("protocols_smoothing_rollVel" ))
  pitchVelSmoother = newExponentialSmoothingT(settings.getValue("protocols_smoothing_pitchVel"))
  yawVelSmoother   = newExponentialSmoothingT(settings.getValue("protocols_smoothing_yawVel"  ))

  rollAccSmoother  = newExponentialSmoothingT(settings.getValue("protocols_smoothing_rollAcc" ))
  pitchAccSmoother = newExponentialSmoothingT(settings.getValue("protocols_smoothing_pitchAcc"))
  yawAccSmoother   = newExponentialSmoothingT(settings.getValue("protocols_smoothing_yawAcc"  ))
end

local function destroy()
  for _,protocol in ipairs(protocols) do
    log("D", "", string.format("Shutdown of protocol %q for vehicle "..objectId.." ("..vehiclePath..") at %s:%d with an update rate of %d Hz (%s)", protocol.name, protocol.ip, protocol.port, protocol.updateRate, tostring(protocol)))
    destroyProtocol(protocol)
  end
end

local protocolsDir = "/lua/vehicle/protocols/"
local filesCache
local function init()
  destroy()
  M.update = nop
  M.updateGFX = nop
  physicsStepUsed = false

  -- optimization, early return without hitting FS
  local outgaugeEnabled = settings.getValue("protocols_outgauge_enabled")
  local motionSimEnabled = settings.getValue("protocols_motionSim_enabled")
  local othersEnabled = settings.getValue("protocols_others_enabled")
  local anyProtocolEnabled = outgaugeEnabled or motionSimEnabled or othersEnabled
  if not anyProtocolEnabled then return end

  local anyProtocolActive = false
  filesCache = filesCache or FS:findFiles(protocolsDir, "*.lua", 0, false, false) -- optimization, do not hit FS for new files (until you ctrl-R)
  for _,filepath in ipairs(filesCache) do
    local filename = filepath:match("[^/]*.lua$")
    local name = filename:sub(0, #filename - 4)
    local enabled = false
    if name == "outgauge" then
      enabled = settings.getValue("protocols_outgauge_enabled")
    elseif name == "motionSim" then
      enabled = settings.getValue("protocols_motionSim_enabled")
    else
      enabled = settings.getValue("protocols_others_enabled")
    end
    if not enabled then
      --log("D", "", "Protocol disabled in configuration, not initializing: "..dumps(name))
      goto continue
    end
    local protocol = initProtocolFromConfig(name)
    if protocol then
      anyProtocolActive = true
      if protocol.physicsStepUsed then
        M.update = updatePhysics
        physicsStepUsed = true
      else
        M.updateGFX = updateGFX
      end
      table.insert(protocols, protocol)
      log("D", "", string.format("Protocol %q active for vehicle "..objectId.." ("..vehiclePath..") at %s:%d with an update rate of %d Hz (%s)", protocol.name, protocol.ip, protocol.port, protocol.updateRate, tostring(protocol)))
    end
    ::continue::
  end
  if anyProtocolActive then
    smoothersInit()
  end
end

local function isPhysicsStepUsed()
  return physicsStepUsed
end

local function settingsChanged()
  init()
end

local function onPlayersChanged(anyPlayerSeated)
  if not playerInfo.firstPlayerSeated then return end
  smoothersReset()
end

M.init = init
M.destroy = destroy
M.settingsChanged = settingsChanged
M.onPlayersChanged = onPlayersChanged

M.reset = reset
M.update = nop
M.updateGFX = nop

M.isPhysicsStepUsed = isPhysicsStepUsed

return M
