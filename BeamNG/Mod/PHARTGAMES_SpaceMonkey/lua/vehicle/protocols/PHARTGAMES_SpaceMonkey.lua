-- This Source Code Form is subject to the terms of the bCDDL, v. 1.1.
-- If a copy of the bCDDL was not distributed with this
-- file, You can obtain one at http://beamng.com/bCDDL-1.1.txt

-- ========================================================================================================================= --
-- For information on how to implement and distribute your custom UDP protocol, please check https://go.beamng.com/protocols --
-- ========================================================================================================================= --

-- generic protocol to guide simple motion platforms
local M = {}

local function init() end
local function reset() end
local function getAddress()        return settings.getValue("protocols_motionSim_address") end        -- return "127.0.0.1"
local function getPort()           return settings.getValue("protocols_motionSim_port") end           -- return 4567
local function getMaxUpdateRate()  return settings.getValue("protocols_motionSim_maxUpdateRate") end  -- return 60

local timeStamp = 0

local function isPhysicsStepUsed()
  --return false-- use graphics step. performance cost is ok. the update rate could reach UP TO min(getMaxUpdateRate(), graphicsFramerate)
  return true   -- use physics step. performance cost is big. the update rate could reach UP TO min(getMaxUpdateRate(), 2000 Hz)
end

local function getStructDefinition()
  return [[
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////// IMPORTANT: if you modify this definition, also update the docs at https://go.beamng.com/protocols /////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    char format[4]; // allows to verify if packet is the expected format, fixed value of "BNG1"
	
    float posX, posY, posZ; // world position of the vehicle
    float velX, velY, velZ; // velocity of the vehicle
    float accX, accY, accZ; // acceleration of the vehicle, gravity not included

    float upX,  upY,  upZ;  // vector components of a vector pointing "up" relative to the vehicle

    float rollPos, pitchPos, yawPos; // angle of roll, pitch and yaw of the vehicle
    float rollVel, pitchVel, yawVel; // angular velocities of roll, pitch and yaw of the vehicle
    float rollAcc, pitchAcc, yawAcc; // angular acceleration of roll, pitch and yaw of the vehicle
	
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
  ]]
end

local function fillStruct(o, dtSim)
  o.format = "BNG2"
  
  timeStamp = timeStamp + dtSim
  o.timeStamp = timeStamp

  o.posX, o.posY, o.posZ = protocols.posX, protocols.posY, protocols.posZ
  o.velX, o.velY, o.velZ = protocols.velXSmoothed, protocols.velYSmoothed, protocols.velZSmoothed
  o.accX, o.accY, o.accZ = protocols.accXSmoothed, protocols.accYSmoothed, protocols.accZSmoothed

  o.upX,  o.upY,  o.upZ  = protocols.upX,  protocols.upY,  protocols.upZ

  o.rollPos, o.pitchPos, o.yawPos = protocols.rollPosSmoothed, protocols.pitchPosSmoothed, protocols.yawPosSmoothed
  o.rollVel, o.pitchVel, o.yawVel = protocols.rollVelSmoothed, protocols.pitchVelSmoothed, protocols.yawVelSmoothed
  o.rollAcc, o.pitchAcc, o.yawAcc = protocols.rollAccSmoothed, protocols.pitchAccSmoothed, protocols.yawAccSmoothed


--engine bits
  local engine = powertrain.getDevice("mainEngine")
  local gearbox = powertrain.getDevice("gearbox")
  local avToRPM = 9.549296596425384

  if engine ~= nil then
    o.engine_rate = engine and engine.outputAV1 * avToRPM or 0
    o.idle_rpm = engine.idleRPM
    o.max_rpm = engine.maxRPM
  end

  if gearbox ~= nil then
	o.gear = gearbox and gearbox.gearIndex or 0
	o.max_gears = gearbox and gearbox.maxGearIndex or 0
  end

  o.suspension_position_bl = 0
  o.suspension_position_br = 0
  o.suspension_position_fl = 0
  o.suspension_position_fr = 0

  o.suspension_velocity_bl = 0
  o.suspension_velocity_br = 0
  o.suspension_velocity_fl = 0
  o.suspension_velocity_fr = 0

  o.suspension_acceleration_bl = 0
  o.suspension_acceleration_br = 0
  o.suspension_acceleration_fl = 0
  o.suspension_acceleration_fr = 0

  o.wheel_speed_bl = 0
  o.wheel_speed_br = 0
  o.wheel_speed_fl = 0
  o.wheel_speed_fr = 0


end

M.init = init
M.reset = reset
M.getAddress = getAddress
M.getPort = getPort
M.getMaxUpdateRate = getMaxUpdateRate
M.getStructDefinition = getStructDefinition
M.fillStruct = fillStruct
M.isPhysicsStepUsed = isPhysicsStepUsed

return M
