using System;
using System.Numerics;

namespace ConsolePong {
    internal class Ball : IDisposable {
        public char symbol = 'O';
        public (int x, int y) renderPos;
        private Vector2 pos;
        private Vector2 moveVector = new();

        public bool killMe { get; private set; } = false;

        private static Random random = new();
        
        // Creates a Ball with random starting momentum.
        public Ball(int x, int y) {
            renderPos.x = x;
            renderPos.y = y;
            pos = new(x, y);

            // get a starting angle
            double angle;
            do {
                angle = (random.NextDouble() * 2 * Math.PI);
            } while ( //only allow flat-ish paths
            angle <= Math.PI/4 || angle >= 7 * Math.PI / 4 ||
            angle >= 3 * Math.PI/4 && angle <= 5 * Math.PI / 4
            );

            // convert angle into 2D vector
            moveVector = new(
                (float)(Math.Cos(angle)),
                (float)(Math.Sin(angle)));
            // scale travel distance per step
            moveVector = Vector2.Divide(moveVector, 3);
            return;
        }

        // move the ball along its trajectory
        public void move() {
            pos = getNextPos();
            renderPos.x = (int)Math.Round(pos.X);
            renderPos.y = (int)Math.Round(pos.Y);
        }

        // return position after the next move() execution
        public Vector2 getNextPos() {
            return Vector2.Add(pos, moveVector);
        }

        // reverse direction on a player's bat, and speed up the ball
        public void reflect_bat() {
            moveVector.Y = -moveVector.Y;
            moveVector = Vector2.Multiply(moveVector, 1.2f);
        }

        // reverse vertical direction when Ball hits the top or bottom borders
        public void reflect_borders() {
            moveVector.X = -moveVector.X;
        }

        public void Dispose() {
            killMe = true;
        }
    }
}