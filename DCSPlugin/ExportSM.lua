--
-- Copyright (c) 2019 Rausch IT
--
-- Permission is hereby granted, free of charge, to any person obtaining a copy 
-- of this software and associated documentation files (the "Software"), to deal
-- in the Software without restriction, including without limitation the rights 
-- to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
-- copies of the Software, and to permit persons to whom the Software is 
-- furnished to do so, subject to the following conditions:
--
-- The above copyright notice and this permission notice shall be included in 
-- all copies or substantial portions of the Software.
--
-- THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
-- IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
-- FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
-- THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
-- LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
-- OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
-- THE SOFTWARE.
--
--
-- SpaceMonkey DCS Lua script to export position and orientation data
--
-- Version 1.0

local udpServer = nil

local t0 = 0


local default_output_file = nil

f_SpaceMonkey = {

	Start = function(self)
		default_output_file = io.open(lfs.writedir().."/Logs/GenericTelemetryProvider.log", "w")

		local version = LoGetVersionInfo() 
		if version and default_output_file then
			default_output_file:write("ProductName: "..version.ProductName..'\n')
			default_output_file:write(string.format("FileVersion: %d.%d.%d.%d\n",
													version.FileVersion[1],
 													version.FileVersion[2],
													version.FileVersion[3],
 													version.FileVersion[4]))
 			default_output_file:write(string.format("ProductVersion: %d.%d.%d.%d\n",
 													version.ProductVersion[1],
 													version.ProductVersion[2],
 													version.ProductVersion[3], 
													version.ProductVersion[4]))
		end
	
	--  Socket
		package.path  = package.path..";"..lfs.currentdir().."/LuaSocket/?.lua"
		package.cpath = package.cpath..";"..lfs.currentdir().."/LuaSocket/?.dll"
		socket = require("socket")
		host = host or "localhost"
		port = port or 6666
	
		udpServer = socket.udp()
		udpServer:setoption('reuseaddr',true)
		udpServer:setpeername(host, port)
	end,

	AfterNextFrame = function(self)
		local curTime = LoGetModelTime()
		if curTime >= t0 then
			-- runs 100 times per second
			t0 = curTime + .01

			local o = LoGetSelfData() -- the ownship data set
			if o then
				local velocity = LoGetVectorVelocity()
				local velX = velocity.x
				local velY = velocity.y
				local velZ = velocity.z
				--local pitch = o.Pitch
				--local yaw = o.Heading
				--local roll = o.Bank


				local pitch, roll, yaw = LoGetADIPitchBankYaw()



				--default_output_file:write(string.format("velocity: %f,%f,%f\n",
				--										velX, velY, velZ))

				--default_output_file:write(string.format("P: %f\n",
				--										pitch))
				--default_output_file:write(string.format("Y: %f\n",
				--										yaw))
				--default_output_file:write(string.format("R: %f\n",
				--									roll))

				if udpServer then
					socket.try(udpServer:send(string.format("%.3f;%.3f;%.3f;%.3f;%.3f;%.3f;%.3f", t0, pitch, yaw, roll, velX, velY, velZ)))
				end
			end
		end
	end,


	Stop = function(self)
	-- Works once just after mission stop.
	
		if default_output_file then
			default_output_file:close()
			default_output_file = nil
		end
	
		if udpServer then
			udpServer:close()
		end
	end

}

----------------------------------------------------------------------------------------------------
--http://forums.eagle.ru/showpost.php?p=2431726&postcount=5
-- Works before mission start
do
  local SimLuaExportStart = LuaExportStart
  LuaExportStart = function()
    f_SpaceMonkey:Start()
    if SimLuaExportStart then
      SimLuaExportStart()
    end
  end
end

-- Works after every simulation frame
do
  local SimLuaExportAfterNextFrame = LuaExportAfterNextFrame
  LuaExportAfterNextFrame = function()
    f_SpaceMonkey:AfterNextFrame()
    if SimLuaExportAfterNextFrame then
      SimLuaExportAfterNextFrame()
    end
  end
end

-- Works after mission stop
do
  local SimLuaExportStop = LuaExportStop
  LuaExportStop = function()
    f_SpaceMonkey:Stop()
    if SimLuaExportStop then
      SimLuaExportStop()
    end
  end
end