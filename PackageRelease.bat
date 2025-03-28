@echo off
setlocal

REM Target directory
set TARGET_DIR=Releases\Working\SpaceMonkey

set WINRAR="C:\Program Files\WinRAR\WinRAR.exe"

REM Copy the directories/files
echo Copying WWReshadeAddon/Release...
robocopy "GenericTelemetryProvider\Release" "%TARGET_DIR%" /E

robocopy "SpaceMonkeyUEVR\Release" "%TARGET_DIR%\SpaceMonkeyUEVR" /E
robocopy "UEVRRelease" "%TARGET_DIR%\SpaceMonkeyUEVR\UEVRRelease" /E
robocopy "CP2077SMMod\Release" "%TARGET_DIR%\CP2077SMMod" /E

echo Copying Readme...
copy "README.md" "%TARGET_DIR%"

REM Compress the directory using WinRAR to SpaceMonkeyRelease.zip
REM Adjust WinRAR path if needed; assuming WinRAR is in PATH
echo Compressing into SpaceMonkeyRelease.zip...

REM Change directory to Releases\Working
pushd "Releases\Working"

REM Compress the WibbleWobble folder into WibbleWobbleRelease.zip
%WINRAR% a -afzip "SpaceMonkeyRelease.zip" "SpaceMonkey"

REM Return to the original directory
popd

echo Done!
endlocal
pause