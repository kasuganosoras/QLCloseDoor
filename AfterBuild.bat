@echo off
set OUTPUT_DIR=%1
echo Copying adb to %OUTPUT_DIR%
if exist "%OUTPUT_DIR%\adb" (
    rmdir /S /Q "%OUTPUT_DIR%\adb"
)
xcopy "adb\" "%OUTPUT_DIR%\adb\" /C /E /Y
