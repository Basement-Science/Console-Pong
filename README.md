# Console-Pong
A quick C# dotnet 5 implementation of Pong that runs in the Terminal. 

![gameplayGIF](/auxFiles/gameplay_animation.gif "gameplayGIF")

## Features
- for 2-Players, unless you want to play against yourself
- Responsive control using Keyboard
- twist: Ball has a chance to split when hit
- highly threaded
- Unicode characters for Graphics
- Windows only (due to Keyboard implementation)

I wrote this over the course of 2 days after having a look at someone else's implementation - that I wasn't happy with. 😏

This could be a good basis if you want to modify it yourself. I probably wont develop this much further.

## Known Issues
- resizing the console breaks everything
- minimum console size required - 120 x 30
- it will display incorrectly unless you use a Unicode Font
- when run in [`Windows Terminal`](https://github.com/Microsoft/Terminal) it may flicker like crazy if your window is not scrolled all the way down. Resize your window AND clear the screen (`cls` in cmd)
- relatively high CPU usage due to high update frequency