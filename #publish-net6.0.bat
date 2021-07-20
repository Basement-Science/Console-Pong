@echo off
color 0A
set dotnetVersion=6.0
call :publish win-x64
call :publish win-x86
timeout 20

goto :end
:publish
dotnet.exe publish --self-contained true --configuration Release --framework net%dotnetVersion% --output .\bin\#publish\net%dotnetVersion%-%1 --runtime %1
::REN ".\bin\#publish\net%dotnetVersion%-%1\ConsolePong*" "ConsolePong-%1*"
:end