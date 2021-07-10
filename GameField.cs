using Pastel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Linq.Enumerable;

namespace ConsolePong {

    public static class Orientation {
        public static bool isAbove(this int a, int b) {
            return a > b;
        }

        public static bool isBelow(this int a, int b) {
            return a < b;
        }

        public enum Direction { UP, DOWN, LEFT, RIGHT }

        public static int getNext(Direction dir, int amount = 1) {
            switch (dir) {
                case Direction.UP:
                    return amount;
                case Direction.DOWN:
                    return -amount;
                case Direction.LEFT:
                    return -amount;
                case Direction.RIGHT:
                    return amount;
                default:
                    throw new ArgumentOutOfRangeException("invalid Enum value");
            }
        }

        public enum TextAlignment { LEFT, CENTER, RIGHT }
        public static StringBuilder OverWrite(this StringBuilder builder, string insert, 
            int startPos, TextAlignment align = TextAlignment.LEFT) {
            switch (align) {
                case TextAlignment.LEFT:
                    return builder.Remove(startPos, insert.Length)
                        .Insert(startPos, insert);
                    break;
                case TextAlignment.CENTER:
                    return builder.Remove(startPos - (insert.Length /2), insert.Length)
                        .Insert(startPos - (insert.Length / 2), insert);
                    break;
                case TextAlignment.RIGHT:
                    return builder.Remove(startPos - insert.Length, insert.Length)
                        .Insert(startPos - insert.Length, insert);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("invalid Enum value");
            }
            
        }
    }

    class GameField {
        char[,] chArray;
        List<Ball> balls = new();
        public Player player_1, player_2;

        private readonly int xStart = 0, xEnd;
        private readonly int yStart = 0, yEnd;

        private readonly int xTop, xBottom;
        private readonly int yLeft, yRight;

        private int xMid;

        private string FieldBorder;
        private (int Left, int Top) ConsolePos_Field, ConsolePos_Score;

        public GameField(int height, int width) {
            chArray = new char[height, width];

            xEnd = height - 1;
            yEnd = width - 1;

            /* BEGIN define where UP-DOWN and LEFT-RIGHT is */
            xTop = Orientation.getNext(Orientation.Direction.UP, xEnd);
            xBottom = 0;
            yLeft = 0;
            yRight = Orientation.getNext(Orientation.Direction.RIGHT, yEnd);
            /* END define where UP-DOWN and LEFT-RIGHT is */

            xMid = chArray.GetLength(0) / 2;
            balls.Add(new Ball(xMid, chArray.GetLength(1) / 2));
            balls.Add(new Ball(xMid, chArray.GetLength(1) / 2));
            player_1 = new(xMid);
            player_2 = new(xMid);

            // init array with spaces
            foreach (int y in Range(yStart, width)) {
                foreach (int x in Range(xStart, height)) {
                    chArray[x, y] = ' ';
                }
            }
            FieldBorder = new string('#', width).Pastel("#FF7800");

            // print header
            Console.WriteLine(new StringBuilder(new string('=', chArray.GetLength(1)))
                .OverWrite("< ConsolePong " +
                FileVersionInfo.GetVersionInfo(
                Assembly.GetExecutingAssembly().Location).ProductVersion.ToString() + $" >", 
                width/2, Orientation.TextAlignment.CENTER).ToString().Pastel("#FFFF00"));

            Console.WriteLine(FieldBorder);
            ConsolePos_Field = Console.GetCursorPosition();
            DrawField();
            Console.WriteLine(FieldBorder);

            ConsolePos_Score = Console.GetCursorPosition();
            PrintScoreBar();
        }

        public void PrintScoreBar() {
            StringBuilder Score = new(new string('=', chArray.GetLength(1)));

            Score.OverWrite(" Player 1 (ctrl: w/s) ", 0, Orientation.TextAlignment.LEFT);
            Score.OverWrite($" {player_1.score} <-- Scores --> {player_2.score} ", Score.Length / 2, Orientation.TextAlignment.CENTER);
            Score.OverWrite(" (ctrl: up / down) Player 2 ", Score.Length, Orientation.TextAlignment.RIGHT);

            Console.SetCursorPosition(ConsolePos_Score.Left, ConsolePos_Score.Top);
            Console.WriteLine(Score.ToString().Pastel("#FFFF00"));
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
                chArray[x, yStart] = isBat(x, player_1) ? '#' : ' ';
                chArray[x, yEnd] = isBat(x, player_2) ? '#' : ' ';

                for (int i = yStart; i.isBelow(yEnd + 1);
                    i = Orientation.getNext(Orientation.Direction.RIGHT, i + 1)) {
                    Console.Write(chArray[x, i]);
                }
                Console.WriteLine();
            }
        }

        public void Move(Player player, Orientation.Direction direction, int amount = 1) {
            switch (direction) {
                case Orientation.Direction.UP:
                    if (!isAtTop(player)) {
                        player.Move(direction, amount);
                    }
                    break;
                case Orientation.Direction.DOWN:
                    if (!isAtBottom(player)) {
                        player.Move(direction, amount);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Cannot move in this direction. ");
            }
        }

        public void ProcessBalls() {
            foreach (var ball in balls) {
                var nextPos = ball.getNextPos();
                int x = Math.Min(Math.Max((int)Math.Round(nextPos.X), xStart), xEnd);
                int y = Math.Min(Math.Max((int)Math.Round(nextPos.Y), yStart), yEnd);

                bool drained = false;

                if (y == yLeft) {
                    if (isBat(x, player_1)) {
                        ball.reflect_bat();
                    } else {
                        drained = true;
                        chArray[ball.renderPos.x, ball.renderPos.y] = ' ';
                        drainedBall(player_2, ball);
                    }
                } else if (y == yRight) {
                    if (isBat(x, player_2)) {
                        ball.reflect_bat();
                    } else {
                        drained = true;
                        chArray[ball.renderPos.x, ball.renderPos.y] = ' ';
                        drainedBall(player_1, ball);
                    }
                }

                if (x == xTop || x == xBottom) {
                    ball.reflect_borders();
                }

                if (!drained) {
                    chArray[ball.renderPos.x, ball.renderPos.y] = ' ';
                    ball.move();
                    chArray[ball.renderPos.x, ball.renderPos.y] = ball.symbol;
                }
            }
            int removed = balls.RemoveAll(b => b.killMe == true);
            while (removed > 0) {
                balls.Add(new Ball(xMid, chArray.GetLength(1) / 2));
                removed--;
            }
        }

        private void drainedBall(Player winner, Ball ball) {
            ball.Dispose();
            winner.addScore();
        }
    }
}
