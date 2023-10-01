-- This Source Code Form is subject to the terms of the bCDDL, v. 1.1.
-- If a copy of the bCDDL was not distributed with this
-- file, You can obtain one at http://beamng.com/bCDDL-1.1.txt

local M = {}


local abs = math.abs

local ffi = require("ffi")

local ip = nil
local port = nil

local updateTime = 0
local updateTimer = 0
local timeStamp = 0

local lastFrameData = nil
local udpSocket = nil

local hasDefinedV1Struct = false
local accX
local accY
local accZ
local accelerationSmoothingX
local accelerationSmoothingY
local accelerationSmoothingZ
local accXSmoother
local accYSmoother
local accZSmoother

local isMotionSimEnabled = false

local avToRPM = 9.549296596425384

M.wheelAccess = {
  frontRight = nil,
  frontLeft = nil,
  rearRight = nil,
  rearLeft = nil
}


local function sendDataPaketV1(dt)
  --log('D', 'motionSim', 'sendDataPaketV1: '..tostring(ip) .. ':' .. tostring(port))

  local o = ffi.new("motionSim_t")
  o.magic = "BNG2"
  
  timeStamp = timeStamp + dt
  
  o.timeStamp = timeStamp

  o.posX, o.posY, o.posZ = obj:getPositionXYZ()

  local velocity = obj:getVelocity()
  o.velX = velocity.x
  o.velY = velocity.y
  o.velZ = velocity.z

  o.accX = accX
  o.accY = accY
  o.accZ = accZ

  local upVector = obj:getDirectionVectorUp()
  local vectorForward = obj:getDirectionVector()

  local quat = quatFromDir(vectorForward, upVector)
  local euler = quat:toEulerYXZ()
  local invQuat = quat:inversed()

  o.upVecX = upVector.x
  o.upVecY = upVector.y
  o.upVecZ = upVector.z

  local rollRate = obj:getRollAngularVelocity()
  local pitchRate = obj:getPitchAngularVelocity()
  local yawRate = obj:getYawAngularVelocity()

  o.rollPos = -euler.z --negated angle here, seems like that is the "standard" for motion sims here
  o.pitchPos = -euler.y --negated angle here, seems like that is the "standard" for motion sims here
  o.yawPos = euler.x

  o.rollRate = rollRate
  o.pitchRate = pitchRate
  o.yawRate = yawRate

  o.rollAcc = (rollRate - lastFrameData.rollRate) / dt
  o.pitchAcc = (pitchRate - lastFrameData.pitchRate) / dt
  o.yawAcc = (yawRate - lastFrameData.yawRate) / dt

  lastFrameData.rollRate = rollRate
  lastFrameData.pitchRate = pitchRate
  lastFrameData.yawRate = yawRate

  -- engine bits
  local engine = powertrain.getDevice("mainEngine")
  local gearbox = powertrain.getDevice("gearbox")

  if engine ~= nil then
    o.engine_rate = engine and engine.outputAV1 * avToRPM or 0
    o.idle_rpm = engine.idleRPM
    o.max_rpm = engine.maxRPM
  end

  if gearbox ~= nil then
	o.gear = gearbox and gearbox.gearIndex or 0
	o.max_gears = gearbox and gearbox.maxGearIndex or 0
  end

  -- suspension bits
  local wheelAccess = M.wheelAccess

  local suspMag = 10

  local wheelPos = vec3(0,0,0)
  if wheelAccess.rearLeft ~= nil then
  	wheelPos = invQuat * vec3(obj:getNodePosition(wheelAccess.rearLeft.node1));
	o.suspension_position_bl = wheelPos.z * suspMag
  end

  if wheelAccess.rearRight ~= nil then
    wheelPos = invQuat * vec3(obj:getNodePosition(wheelAccess.rearRight.node1));
    o.suspension_position_br = wheelPos.z * suspMag
  end

  if wheelAccess.frontLeft ~= nil then
    wheelPos = invQuat * vec3(obj:getNodePosition(wheelAccess.frontLeft.node1));
    o.suspension_position_fl = wheelPos.z * suspMag
  end

  if wheelAccess.frontRight ~= nil then
  	  wheelPos = invQuat * vec3(obj:getNodePosition(wheelAccess.frontRight.node1));
	  o.suspension_position_fr = wheelPos.z * suspMag
  end

  o.suspension_velocity_bl = (o.suspension_position_bl - lastFrameData.suspension_position_bl ) / dt
  o.suspension_velocity_br = (o.suspension_position_br - lastFrameData.suspension_position_br ) / dt
  o.suspension_velocity_fl = (o.suspension_position_fl - lastFrameData.suspension_position_fl ) / dt
  o.suspension_velocity_fr = (o.suspension_position_fr - lastFrameData.suspension_position_fr ) / dt

  o.suspension_acceleration_bl = (o.suspension_velocity_bl - lastFrameData.suspension_velocity_bl ) / dt
  o.suspension_acceleration_br = (o.suspension_velocity_br - lastFrameData.suspension_velocity_br ) / dt
  o.suspension_acceleration_fl = (o.suspension_velocity_fl - lastFrameData.suspension_velocity_fl ) / dt
  o.suspension_acceleration_fr = (o.suspension_velocity_fr - lastFrameData.suspension_velocity_fr ) / dt

  if wheelAccess.rearLeft ~= nil and wheelAccess.rearRight ~= nil and wheelAccess.frontLeft  ~= nil and wheelAccess.frontRight ~= nil then
	  o.wheel_speed_bl = abs(wheelAccess.rearLeft.angularVelocity * wheelAccess.rearLeft.radius);
	  o.wheel_speed_br = abs(wheelAccess.rearRight.angularVelocity * wheelAccess.rearRight.radius);
	  o.wheel_speed_fl = abs(wheelAccess.frontLeft.angularVelocity * wheelAccess.frontLeft.radius);
	  o.wheel_speed_fr = abs(wheelAccess.frontRight.angularVelocity * wheelAccess.frontRight.radius);
  end


  lastFrameData.suspension_position_bl = o.suspension_position_bl;
  lastFrameData.suspension_position_br = o.suspension_position_br;
  lastFrameData.suspension_position_fl = o.suspension_position_fl;
  lastFrameData.suspension_position_fr = o.suspension_position_fr;

  lastFrameData.suspension_velocity_bl = o.suspension_velocity_bl;
  lastFrameData.suspension_velocity_br = o.suspension_velocity_br;
  lastFrameData.suspension_velocity_fl = o.suspension_velocity_fl;
  lastFrameData.suspension_velocity_fr = o.suspension_velocity_fr;

  lastFrameData.wheel_speed_bl = o.wheel_speed_bl;
  lastFrameData.wheel_speed_br = o.wheel_speed_br;
  lastFrameData.wheel_speed_fl = o.wheel_speed_fl;
  lastFrameData.wheel_speed_fr = o.wheel_speed_fr;

  -- if streams.willSend("profilingData") then
  --   gui.send(
  --     "genericGraphAdvanced",
  --     {
  --       --
  --       accX = {title = "Acc X", color = getContrastColorStringRGB(1), unit = "", value = o.accX},
  --       accY = {title = "Acc Y", color = getContrastColorStringRGB(2), unit = "", value = o.accY},
  --       accZ = {title = "Acc Z", color = getContrastColorStringRGB(3), unit = "", value = o.accZ}
  --     }
  --   )
  -- end

  --convert the struct into a string
  local packet = ffi.string(o, ffi.sizeof(o))

  --log("I", "motionSim.sendDataPaketV1", "Sending To: " .. ip .. "port: " .. port)
  udpSocket:sendto(packet, ip, port)
end

local function updateGFXV1(dt)
end

local function updateV1(dt)
  if not playerInfo.firstPlayerSeated then
    return
  end

  updateTimer = updateTimer + dt
  local accXRaw = -obj:getSensorX()
  local accYRaw = -obj:getSensorY()
  local accZRaw = -obj:getSensorZ()
  accX = accelerationSmoothingX > 0 and accXSmoother:get(accXRaw) or accXRaw
  accY = accelerationSmoothingY > 0 and accYSmoother:get(accYRaw) or accYRaw
  accZ = accelerationSmoothingZ > 0 and accZSmoother:get(accZRaw) or accZRaw

  if updateTimer >= updateTime then
    sendDataPaketV1(updateTimer)
    updateTimer = 0
  end
end

local function resetV1()
  lastFrameData = {
    rollRate = 0,
    pitchRate = 0,
    yawRate = 0,
    suspension_position_bl = 0,
    suspension_position_br = 0,
    suspension_position_fl = 0,
    suspension_position_fr = 0,
    suspension_velocity_bl = 0,
    suspension_velocity_br = 0,
    suspension_velocity_fl = 0,
    suspension_velocity_fr = 0,
	wheel_speed_bl = 0,
	wheel_speed_br = 0,
	wheel_speed_fl = 0,
	wheel_speed_fr = 0
  }

  accXSmoother:reset()
  accYSmoother:reset()
  accZSmoother:reset()
end

local function settingsChanged()
  M.onExtensionLoaded()
end

local function initV1()
  if not hasDefinedV1Struct then
    ffi.cdef [[
    typedef struct motionSim_t  {
      //Magic to check if packet is actually useful, fixed value of "BNG2"
      char           magic[4];

      //World position of the car
      float          posX;
      float          posY;
      float          posZ;

      //Velocity of the car
      float          velX;
      float          velY;
      float          velZ;

      //Acceleration of the car, gravity not included
      float          accX;
      float          accY;
      float          accZ;

      //Vector components of a vector pointing "up" relative to the car
      float          upVecX;
      float          upVecY;
      float          upVecZ;

      //Roll, pitch and yaw positions of the car
      float          rollPos;
      float          pitchPos;
      float          yawPos;

      //Roll, pitch and yaw "velocities" of the car
      float          rollRate;
      float          pitchRate;
      float          yawRate;

      //Roll, pitch and yaw "accelerations" of the car
      float          rollAcc;
      float          pitchAcc;
      float          yawAcc;
      
      //engine
      float engine_rate;
      float idle_rpm;
      float max_rpm;

      //gears
      int gear;
      int max_gears;      

      //suspension 
      float suspension_position_bl;
      float suspension_position_br;
      float suspension_position_fl;
      float suspension_position_fr;

      float suspension_velocity_bl;
      float suspension_velocity_br;
      float suspension_velocity_fl;
      float suspension_velocity_fr;

      float suspension_acceleration_bl;
      float suspension_acceleration_br;
      float suspension_acceleration_fl;
      float suspension_acceleration_fr;
	  
	  float wheel_speed_bl;
	  float wheel_speed_br;
	  float wheel_speed_fl;
	  float wheel_speed_fr;

	  float timeStamp;

    } motionSim_t;
    ]]
    hasDefinedV1Struct = true
  end

  lastFrameData = {
    rollRate = 0,
    pitchRate = 0,
    yawRate = 0,
    suspension_position_bl = 0,
    suspension_position_br = 0,
    suspension_position_fl = 0,
    suspension_position_fr = 0,
    suspension_velocity_bl = 0,
    suspension_velocity_br = 0,
    suspension_velocity_fl = 0,
    suspension_velocity_fr = 0,
	wheel_speed_bl = 0,
	wheel_speed_br = 0,
	wheel_speed_fl = 0,
	wheel_speed_fr = 0
  }

  ip = settings.getValue("motionSimIP") or "127.0.0.1"
  port = settings.getValue("motionSimPort") or 4444
  local updateRate = settings.getValue("motionSimHz") or 100
  updateTime = 1 / updateRate

  accelerationSmoothingX = settings.getValue("motionSimAccelerationSmoothingX") or 30
  accelerationSmoothingY = settings.getValue("motionSimAccelerationSmoothingY") or 30
  accelerationSmoothingZ = settings.getValue("motionSimAccelerationSmoothingZ") or 30
  accXSmoother = newExponentialSmoothing(accelerationSmoothingX)
  accYSmoother = newExponentialSmoothing(accelerationSmoothingY)
  accZSmoother = newExponentialSmoothing(accelerationSmoothingZ)

  log("I", "motionSim.initV1", string.format("SpaceMonkey MotionSim V1 active! IP config: %s:%d, update rate: %dhz", ip, port, updateRate))

  udpSocket = socket.udp()

  M.update = updateV1
  M.updateGFX = updateGFXV1
  M.reset = resetV1
end

local function onExtensionLoaded()
  M.reset = nop
  M.updateGFX = nop
  M.update = nop

    for _, v in pairs(wheels.wheels) do --Todo: fix up
    if v.name == "FR" then
      M.wheelAccess.frontRight = v
    elseif v.name == "FL" then
      M.wheelAccess.frontLeft = v
    elseif v.name == "RR" then
      M.wheelAccess.rearRight = v
    elseif v.name == "RL" then
      M.wheelAccess.rearLeft = v
    end
  end


  isMotionSimEnabled = settings.getValue("motionSimEnabled") or false
  if isMotionSimEnabled then
    local motionSimVersion = settings.getValue("motionSimVersion") or 1
    log("I", "motionSim.onExtensionLoaded", "Trying to load SpaceMonkey motionSim with version: " .. motionSimVersion)
    if motionSimVersion == 1 then
      log("D", "motionSim.onExtensionLoaded", "SpaceMonkey motionSim active!" .. motionSimVersion)
      initV1()
    else
      log("E", "motionSim.onExtensionLoaded", "Unknown SpaceMonkey motionSim version: " .. motionSimVersion)
    end
  end
end

local function isPhysicsStepUsed()
  return isMotionSimEnabled
end

M.onExtensionLoaded = onExtensionLoaded
M.reset = nop
M.settingsChanged = settingsChanged

M.updateGFX = nop
M.update = nop

M.isPhysicsStepUsed = isPhysicsStepUsed


return M
