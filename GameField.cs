using Pastel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Linq.Enumerable;

namespace ConsolePong {
    class GameField {
        public class TextArea {
            private readonly StringBuilder ClearLine;
            private char[,] chArray; // shared with Gamefield instance
            private string lastLine = "";

            int TextAreaWidth, TextAreaHeight;

            int line;
            public TextArea(int width, int height, ref char[,] chArray) {
                TextAreaWidth = width - 4;
                TextAreaHeight = height - 2;
                ClearLine = new StringBuilder(new string(' ', TextAreaWidth));
                this.chArray = chArray;
                line = TextAreaHeight;
            }

            public void ClearField() {
                int startLine = line;
                do {
                    WriteToField(""); // increments line
                } while (line != startLine);
            }

            public void AppendToLastLine(string text) {
                WriteToField(lastLine + text, true);
            }

            public void WriteToField(string text, bool overWriteLast = false) {
                if (overWriteLast) {
                    line = line + 1 <= TextAreaHeight ? line + 1 : 0;
                }

                foreach (var line in text.Split('\n')) {
                    if (line.Length > TextAreaWidth) {
                        var words = line.Split(' ', StringSplitOptions.None);
                        var rearrangedLine = new StringBuilder();
                        foreach (var word in words) {
                            if (rearrangedLine.Length + word.Length <= TextAreaWidth) {
                                rearrangedLine.Append(word);
                                if (rearrangedLine.Length+1 <= TextAreaWidth) {
                                    rearrangedLine.Append(' ');
                                }
                            } else {
                                WriteLine(rearrangedLine.ToString());
                                rearrangedLine.Clear().Append(word).Append(' ');
                            }
                        }
                        if (rearrangedLine.Length > 0) {
                            WriteLine(rearrangedLine.ToString());
                        }
                    } else {
                        WriteLine(line);
                    }

                }
            }

            private void WriteLine(string text) {
                lastLine = text;
                StringBuilder output = new StringBuilder(ClearLine.ToString())
                    .OverWrite(text, TextAreaWidth/2, Misc.TextAlignment.CENTER);
                var arr = output.ToString().ToCharArray();
                for (int x = 0; x < TextAreaWidth; x++) {
                    chArray[line +1, x + 2] = arr[x];
                }
                line = line <= 0 ? TextAreaHeight : line - 1;
            }
        } // end TextArea

        char[,] chArray; // shared with TextArea instance
        List<Ball> balls = new();
        public Player player_1, player_2;

        public readonly int xStart = 0, xEnd;
        public readonly int yStart = 0, yEnd;

        private readonly int xTop, xBottom;
        private readonly int yLeft, yRight;

        private int xMid;

        private readonly string FieldBorder;
        private readonly string ClearLine;

        private (int Left, int Top) ConsolePos_Field, ConsolePos_Score;
        public TextArea textArea;

        private const char batChar = '█'; // unicode full block
        private const char borderChar = '█'; // unicode full block
        private const char headerChar = '═'; // unicode box drawing doubleLine char

        private readonly Color borderColor = Color.DarkOrange;
        private readonly Color textColor = Color.Yellow;
        private readonly Color fieldColor = Color.RoyalBlue;

        public GameField(int height, int width) {
            chArray = new char[height, width];

            xEnd = height - 1;
            yEnd = width - 1;

            /* BEGIN define where UP-DOWN and LEFT-RIGHT is */
            xTop = Misc.getNext(Misc.Direction.UP, xEnd);
            xBottom = 0;
            yLeft = 0;
            yRight = Misc.getNext(Misc.Direction.RIGHT, yEnd);
            /* END define where UP-DOWN and LEFT-RIGHT is */

            xMid = chArray.GetLength(0) / 2;
            balls.Add(new Ball(this, xMid, chArray.GetLength(1) / 2));
            player_1 = new(xMid);
            player_2 = new(xMid);

            // init array with spaces
            foreach (int y in Range(yStart, width)) {
                foreach (int x in Range(xStart, height)) {
                    chArray[x, y] = ' ';
                }
            }
            FieldBorder = new string(borderChar, width).Pastel(borderColor);
            ClearLine = new string(' ', width);

            // print header
            Console.WriteLine(new StringBuilder(new string(headerChar, chArray.GetLength(1)))
                .OverWrite("< ConsolePong " + 
                Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion + $" >", 
                width/2, Misc.TextAlignment.CENTER).ToString().Pastel(textColor));

            Console.WriteLine(FieldBorder);
            ConsolePos_Field = Console.GetCursorPosition();
            textArea = new(width, height, ref chArray);

            DrawField();
            Console.WriteLine(FieldBorder);

            ConsolePos_Score = Console.GetCursorPosition();
            PrintScoreBar();
        }

        public void PrintScoreBar() {
            PrintScoreBarAt(ConsolePos_Score);
        }

        private void PrintScoreBarAt((int Left, int Top) consolePosition) {
            Console.SetCursorPosition(consolePosition.Left, consolePosition.Top);
            Console.WriteLine(getScoreBarString().Pastel(textColor));
        }

        private string getScoreBarString() {
            StringBuilder Score = new(new string(headerChar, chArray.GetLength(1)));

            Score.OverWrite(" Player 1 (ctrl: W/S) ", 0, Misc.TextAlignment.LEFT);
            Score.OverWrite($" {player_1.score} <-- Scores --> {player_2.score} ", Score.Length / 2, Misc.TextAlignment.CENTER);
            Score.OverWrite(" (ctrl: up/down) Player 2 ", Score.Length, Misc.TextAlignment.RIGHT);
            return Score.ToString();
        }

        public void ClearScreen() {
            // start at the Border
            ConsolePos_Field.Top -= 1;
            PrintScoreBarAt(ConsolePos_Field);
            // clear the remaining Lines.
            for (int line = 0; line < yEnd + 2; line++) {
                Console.WriteLine(ClearLine);
            }
            // finally set curser position so we can exit
            Console.SetCursorPosition(ConsolePos_Field.Left, ConsolePos_Field.Top+1);
        }

        private bool isAtTop(Player player) {
            return player.xTop == xTop;
        }

        private bool isAtBottom(Player player) {
            return player.xBottom == xBottom;
        }

        private bool isBat(int x, Player player) {
            return (!x.isAbove(player.xTop)) && (!x.isBelow(player.xBottom));
        }
        public void DrawField() {
            Console.SetCursorPosition(ConsolePos_Field.Left, ConsolePos_Field.Top);
            for (int x = xEnd; x >= xStart; x--) {
                chArray[x, yStart] = isBat(x, player_1) 
                    ? batChar : chArray[x, yStart] == batChar ? ' ' : chArray[x, yStart];
                chArray[x, yEnd] = isBat(x, player_2) 
                    ? batChar : chArray[x, yEnd] == batChar ? ' ' : chArray[x, yEnd];

                for (int y = yStart; y.isBelow(yEnd + 1);
                    y = Misc.getNext(Misc.Direction.RIGHT, y + 1)) {
                    Console.Write(chArray[x, y].ToString().Pastel(fieldColor));
                }
                Console.WriteLine();
            }
        }

        public void Move(Player player, Misc.Direction direction) {
            switch (direction) {
                case Misc.Direction.UP:
                    if (!isAtTop(player)) {
                        player.Move(direction, 1);
                    }
                    break;
                case Misc.Direction.DOWN:
                    if (!isAtBottom(player)) {
                        player.Move(direction, 1);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Cannot move in this direction. ");
            }
        }

        public void ProcessBalls() {
            foreach (var ball in new List<Ball>(balls)) {
                var nextPos = ball.getNextPos();
                int x = Misc.boundValue(nextPos.X, xStart, xEnd);
                int y = Misc.boundValue(nextPos.Y, yStart, yEnd);

                if (y == yLeft) {
                    if (isBat(x, player_1)) {
                        hitBat();
                    } else {
                        chArray[ball.renderPos.x, ball.renderPos.y] = ' ';
                        drainedBall(player_2, ball, x, y);
                        continue;
                    }
                } else if (y == yRight) {
                    if (isBat(x, player_2)) {
                        hitBat();
                    } else {
                        chArray[ball.renderPos.x, ball.renderPos.y] = ' ';
                        drainedBall(player_1, ball, x, y);
                        continue;
                    }
                }
                void hitBat() {
                    ball.reflect_bat();

                    if (Misc.random.Next(100) >= 50) {
                        balls.Add(ball.split());
                    }
                }

                if (x == xTop || x == xBottom) {
                    ball.reflect_borders();
                }
                chArray[ball.renderPos.x, ball.renderPos.y] = ' ';
                ball.move();
                chArray[ball.renderPos.x, ball.renderPos.y] = ball.symbol;
            } // end foreach
        }

        private void drainedBall(Player winner, Ball ball, int x, int y) {
            ball.Dispose();
            winner.addScore();

            chArray[x, y] = (char)183; // middle dot ·
            balls.Remove(ball);

            if (balls.Count == 0) {
                balls.Add(new Ball(this, xMid, chArray.GetLength(1) / 2));
            }
        }
    }
}
