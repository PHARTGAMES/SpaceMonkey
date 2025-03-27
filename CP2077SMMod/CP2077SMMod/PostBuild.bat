pushd %~dp0
set FROM_DIR=%1
set PLATFORM=%2
set TO_DIR_CP="D:\Program Files (x86)\Steam\steamapps\common\Cyberpunk 2077\red4ext\plugins"

if "%PLATFORM%"=="Win32" (
	set TO_DIR_LIBS="..\Release\Win32"
    set PLT=32
) else (
	set TO_DIR_LIBS="..\Release\x64"
    set PLT=
)

copy /Y "%FROM_DIR%SpaceMonkeyTelemetryAPI%PLT%.dll" %TO_DIR_LIBS%
copy /Y "%FROM_DIR%CP2077SMMod%PLT%.dll" %TO_DIR_LIBS%

copy /Y "%FROM_DIR%SpaceMonkeyTelemetryAPI%PLT%.dll" %TO_DIR_CP%
copy /Y "%FROM_DIR%CP2077SMMod%PLT%.dll" %TO_DIR_CP%

copy /Y "%FROM_DIR%SpaceMonkeyTelemetryAPI%PLT%.dll" "..\..\GenericTelemetryProvider\Release"

popd