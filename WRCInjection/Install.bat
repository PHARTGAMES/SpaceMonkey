@echo off

if not exist "PhysXCooking64_s.dll" (
	echo "PhysXCooking64_s.dll" not found.
	echo ERROR: Unzip "wrc-telemetry.zip" in WRC's installation folder then run this file again.
	pause
	exit
)

if not exist "WrcInjectionPayload.dll" (
	echo "WrcInjectionPayload.dll" not found.
	echo ERROR: Unzip "wrc-telemetry.zip" in WRC's installation folder then run this file again.
	pause
	exit
)

if exist "PhysXCooking64_s_org.dll" (
	echo "PhysXCooking64_s_org.dll" found.
	echo Reverting previous patch.

	copy PhysXCooking64_s_org.dll PhysXCooking64_s.dll

	if errorlevel 1 (
		echo ERROR: Is WRC running? If so, please close WRC and try again.
		pause
		exit
	)
)

echo Installing patch.

copy PhysXCooking64_s.dll PhysXCooking64_s_org.dll
copy WrcInjectionPayload.dll PhysXCooking64_s.dll

if errorlevel 1 (
	echo ERROR: Is WRC running? If so, please close WRC and try again.
	pause
	exit
)

echo Done. You may close this window.
pause
