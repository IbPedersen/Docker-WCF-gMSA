@echo off

set PORT=60000

set HOST=localhost
set /P HOST=Enter host (press Enter for '%HOST%') : 

set ENDPOINT_ID=
set /P ENDPOINT_ID=Optional set identity for service endpoint (UPN) : 
if not "%ENDPOINT_ID%" == "" set UPN=-upn="%ENDPOINT_ID%"
if "%ENDPOINT_ID%" == "-" set UPN=-upn=

@echo on
%~dp0image\WCF.exe -client=%HOST%:%PORT% %UPN%
@echo off

:end
pause
