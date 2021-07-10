@echo off
color 0A
call :publish win-x64
call :publish win-x86
timeout 20

goto :end
:publish
dotnet.exe publish --self-contained true --configuration Release --framework net6.0 --output .\bin\#publish\net6.0-%1 --runtime %1
REN ".\bin\#publish\net6.0-%1\ConsolePong*" "ConsolePong-%1*"
:end