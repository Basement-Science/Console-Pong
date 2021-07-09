using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolePong {
    class GameField {
        char[,] chArray;
        Ball ball;
        public Player player_1;
        public Player player_2;

        private int xTop;
        private int xBottom;

        public GameField(int height, int width) {
            chArray = new char[height, width];
            int xMid = chArray.GetLength(0) / 2;
            ball = new(xMid, chArray.GetLength(1) / 2);
            player_1 = new(xMid);
            player_2 = new(xMid);

            xTop = height - 1;
            xBottom = 0;
        }

        public void Move(Player player, Direction direction) {
            switch (direction) {
                case Direction.UP:
                    if (! isAtTop(player)) {
                        player.Move(direction);
                    }
                    break;
                case Direction.DOWN:
                    if (!isAtBottom(player)) {
                        player.Move(direction);
                    }
                    break;
            }
        }

        public enum Direction { UP, DOWN }

        private bool isAtTop(Player player) {
            return player.xTop == xTop;
        }

        private bool isAtBottom(Player player) {
            return player.xBottom == xBottom;
        }
    }
}
