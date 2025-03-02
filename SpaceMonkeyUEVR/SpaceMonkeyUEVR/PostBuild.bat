pushd %~dp0
set FROM_DIR=%1
set PLATFORM=%2
set TO_DIR_PLUGS="%APPDATA%\UnrealVRMod\TrailOut-Win64-Shipping\plugins\"

if "%PLATFORM%"=="Win32" (
	set TO_DIR_LIBS="..\Release\Win32"
    set PLT=32
) else (
	set TO_DIR_LIBS="..\Release\x64"
    set PLT=
)

copy /Y "%FROM_DIR%SpaceMonkeyUEVR%PLT%.dll" %TO_DIR_LIBS%
copy /Y "%FROM_DIR%SpaceMonkeyUEVR%PLT%.dll" %TO_DIR_PLUGS%

popd