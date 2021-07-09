using Pastel;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }

    class GameField {
        char[,] chArray;
        Ball ball;
        public Player player_1, player_2;

        private readonly int xStart = 0, xEnd;
        private readonly int yStart = 0, yEnd;

        private readonly int xTop, xBottom;
        private readonly int yLeft, yRight;

        private string FieldBorder = new("");
        private (int Left, int Top) ConsolePos;

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

            int xMid = chArray.GetLength(0) / 2;
            ball = new(xMid, chArray.GetLength(1) / 2);
            player_1 = new(xMid);
            player_2 = new(xMid);

            // init array with spaces
            foreach (int y in Range(yStart, width)) {
                foreach (int x in Range(xStart, height)) {
                    chArray[x, y] = ' ';
                }
                FieldBorder += '#';
            }
            FieldBorder = FieldBorder.Pastel("#FF7800");
            Console.WriteLine(FieldBorder);
            ConsolePos = Console.GetCursorPosition();
            DrawField();
            Console.WriteLine(FieldBorder);
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
            Console.SetCursorPosition(ConsolePos.Left, ConsolePos.Top);
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
    }
}
