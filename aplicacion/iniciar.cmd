@echo off
setlocal
if not exist "%~dp0bin\SolicitudesRpa.exe" (
  echo Primero ejecute compilar.ps1 desde PowerShell. Consulte README.md.
  pause
  exit /b 1
)
start "" "%~dp0bin\SolicitudesRpa.exe"
