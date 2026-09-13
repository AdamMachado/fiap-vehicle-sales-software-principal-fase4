@echo off
setlocal
chcp 65001 >nul

set "SCRIPT_DIR=%~dp0"

where pwsh.exe >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%check-coverage.ps1" %*
) else (
    powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%check-coverage.ps1" %*
)

exit /b %ERRORLEVEL%
