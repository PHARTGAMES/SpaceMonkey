@echo off
set "batch_dir=%~dp0"
set "install_path=%batch_dir%"

:: Add wibblewobble install path registry entry for 32-bit processes
reg add "HKLM\SOFTWARE\WOW6432Node\PHARTGAMES\SpaceMonkeyTP" /v "install_path" /t REG_SZ /d "%install_path%\"Release\

:: Add wibblewobble install path registry entry for 64-bit processes
reg add "HKLM\SOFTWARE\PHARTGAMES\SpaceMonkeyTP" /v "install_path" /t REG_SZ /d "%install_path%\"Release\


echo "If you see Access Denied run Register.bat as administrator"
pause