using System;
using System.Text;

namespace ConsolePong {
    public static class Misc{
        public static Random random = new();

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
                case TextAlignment.CENTER:
                    return builder.Remove(startPos - (insert.Length /2), insert.Length)
                        .Insert(startPos - (insert.Length / 2), insert);
                case TextAlignment.RIGHT:
                    return builder.Remove(startPos - insert.Length, insert.Length)
                        .Insert(startPos - insert.Length, insert);
                default:
                    throw new ArgumentOutOfRangeException("invalid Enum value");
            }
            
        }
        public static int boundValue(double value, int start, int end) {
            return Math.Min(Math.Max((int)Math.Round(value), start), end);
        }
    }
}
