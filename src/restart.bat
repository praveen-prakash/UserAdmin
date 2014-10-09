@echo off
staradmin kill all

REM Start Launcher
call ..\..\Polyjuice\Launcher\run.bat

REM Start Sign-In App
call ..\..\Polyjuice\SignInApp\src\run.bat

REM Start User Admin App
call %~dp0run.bat