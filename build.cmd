@echo off

set LOCAL_VERSION=1903
set AKS_VERSION=10.0.17763.737

:version
echo.
set WIN_VERSIONNO=
set /P WIN_VERSIONNO= Select  1 = %LOCAL_VERSION% (local),  2 = %AKS_VERSION% (AKS): 
set WIN_VERSION=
if "%WIN_VERSIONNO%" == "1" (set WIN_VERSION=%LOCAL_VERSION%)
if "%WIN_VERSIONNO%" == "2" (set WIN_VERSION=%AKS_VERSION%)
if "%WIN_VERSION" == "" goto version

rem call :build windows %WIN_VERSION% win
call :build windows/servercore %WIN_VERSION% core

goto end


:build
echo.
echo Using: FROM mcr.microsoft.com/%1:%2
echo on
docker build --build-arg WIN_IMAGE=%1 --build-arg WIN_VERSION=%2 -f %~dp0\Dockerfile -t simcorp/wcf_%3_%2 ./image
@echo off
exit /b

:end
pause
