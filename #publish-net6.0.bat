@echo off
color 0A
call :publish win-x64
timeout 20

goto :end
:publish
dotnet.exe publish --self-contained true --configuration Release --framework net6.0 --output .\bin\#publish\net6.0-%1 --runtime %1
REN ".\bin\#publish\net6.0-%1\Timestamp*" "Timestamp-%1*"
:end