@echo off

set WIN_VERSION=1903
set PORT=60000

set GMSA=
set /P GMSA=Enter filename of gMSA file. E.g. gMSAAideTfUsr.json : 

set SEC_OPT=
if not "%GMSA%" == "" set SEC_OPT=--security-opt "credentialspec=file://%GMSA%"

set ENDPOINT_ID=
set /P ENDPOINT_ID=Optional set identity for service endpoint (UPN) : 
if not "%ENDPOINT_ID%" == "" set UPN=-upn="%ENDPOINT_ID%"
if "%ENDPOINT_ID%" == "-" set UPN=-upn=

@echo on
docker run --rm -p %PORT%:%PORT% -it --name wcf_server -h WCF_SRV %SEC_OPT% simcorp/wcf_core_%WIN_VERSION% WCF.exe -server=%PORT% %UPN%
@echo off

:end
pause
