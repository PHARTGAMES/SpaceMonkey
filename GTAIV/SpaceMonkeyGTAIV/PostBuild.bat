pushd %~dp0
set FROM_DIR=%1
set PLATFORM=%2
set TO_DIR_GTAIV="D:\SteamLibrary\steamapps\common\Grand Theft Auto IV\GTAIV"
set TO_DIR_REL="..\Release\"

copy /Y "%FROM_DIR%SpaceMonkeyTelemetryAPI32.dll" %TO_DIR_REL%
copy /Y "%FROM_DIR%SpaceMonkeyGTAIV.asi" %TO_DIR_REL%
copy /Y "%FROM_DIR%SMXInputFFBHost.dll" %TO_DIR_REL%
copy /Y "%FROM_DIR%SMXInputFFBClient.dll" %TO_DIR_REL%
copy /Y "%FROM_DIR%CMCustomUDPNative.dll" %TO_DIR_REL%

copy /Y "%FROM_DIR%SpaceMonkeyTelemetryAPI32.dll" %TO_DIR_GTAIV%
copy /Y "%FROM_DIR%SpaceMonkeyGTAIV.asi" %TO_DIR_GTAIV%
copy /Y "%FROM_DIR%SMXInputFFBHost.dll" %TO_DIR_GTAIV%
copy /Y "%FROM_DIR%SMXInputFFBClient.dll" %TO_DIR_GTAIV%
copy /Y "%FROM_DIR%CMCustomUDPNative.dll" %TO_DIR_GTAIV%

copy /Y "%FROM_DIR%SpaceMonkeyTelemetryAPI32.dll" "..\..\GenericTelemetryProvider\Release"

popd
