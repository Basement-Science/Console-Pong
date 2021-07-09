using System;
using System.Numerics;

namespace ConsolePong {
    internal class Ball {
        private int x;
        private int y;
        private char symbol = 'O';
        Vector2 moveVector = new();

        private static Random random = new();
        
        public Ball(int x, int y) {
            this.x = x;
            this.y = y;

            double angle = random.NextDouble() * Math.PI;
            moveVector.X = (float)(Math.Cos(angle) - 1.0 * Math.Sin(angle));
            moveVector.Y = (float)(Math.Sin(angle) + 1.0 * Math.Cos(angle));
        }
    }
}