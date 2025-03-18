@echo off
setlocal

call CleanupRelease.bat

call "./Build/BuildRelease.bat"

pushd "%~dp0"
call PackageRelease.bat
popd

endlocal
