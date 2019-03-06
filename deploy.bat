@ECHO OFF
ECHO Start Deploy
powershell.exe Stop-Process -Name "3wBetManager-API" -passThru -ErrorAction silentlycontinue
start 3wBetManager-API\bin\Release\3wBetManager-API.exe