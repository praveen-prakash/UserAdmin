@echo off
staradmin kill all

REM Start Launcher
call "%~dp0..\..\Launcher\run.bat"

REM Start Sign-In App
call "%~dp0..\..\SignInApp\src\run.bat"

REM Start User Admin App
call "%~dp0run.bat"