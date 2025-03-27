pushd %~dp0
set FROM_DIR=%1
set PLATFORM=%2
set TO_DIR_CP_PLUGINS="D:\Program Files (x86)\Steam\steamapps\common\Cyberpunk 2077\red4ext\plugins"
set TO_DIR_CP_BIN="D:\Program Files (x86)\Steam\steamapps\common\Cyberpunk 2077\bin\x64"
set TO_DIR_REL_PLUGINS="..\Release\red4ext\plugins"
set TO_DIR_REL_BIN="..\Release\bin\x64"

copy /Y "%FROM_DIR%SpaceMonkeyTelemetryAPI.dll" %TO_DIR_REL_BIN%
copy /Y "%FROM_DIR%CP2077SMMod.dll" %TO_DIR_REL_PLUGINS%

copy /Y "%FROM_DIR%SpaceMonkeyTelemetryAPI.dll" %TO_DIR_CP_BIN%
copy /Y "%FROM_DIR%CP2077SMMod.dll" %TO_DIR_CP_PLUGINS%

copy /Y "%FROM_DIR%SpaceMonkeyTelemetryAPI.dll" "..\..\GenericTelemetryProvider\Release"

popd