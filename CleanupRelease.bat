@echo off
setlocal

REM Target directory
set TARGET_DIR=Releases\Working\SpaceMonkey

REM If target directory exists, remove it
if exist "%TARGET_DIR%" (
    echo Removing old target directory contents...
    rd /s /q "%TARGET_DIR%"
)

REM Remove old zip file if it exists
if exist "Releases\Working\SpaceMonkeyRelease.zip" (
    echo Removing old zip file...
    del "Releases\Working\SpaceMonkeyRelease.zip"
)

REM Create the target directory structure
echo Creating target directory...
mkdir "%TARGET_DIR%"
endlocal
exit /b 0

