using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsolePong {
    class Program {
        private static GameField gameField = new(25, 120);
        static async Task Main(string[] args) {
            var inputHandler = new Task(ReadKeys);
            var outputHandler = new Task(PrintGameField);

            Console.CancelKeyPress += new ConsoleCancelEventHandler(ExitHandler);
            Console.CursorVisible = false;

            inputHandler.Start();
            outputHandler.Start();
            while (true) { }
        }

        private static void ExitHandler(object sender, ConsoleCancelEventArgs e) {
            e.Cancel = true;
        }

        static void ReadKeys() {
            for (ConsoleKeyInfo keyInfo = new(); 
                keyInfo.Key != ConsoleKey.Escape;
                keyInfo = Console.ReadKey(true)) {

                switch (keyInfo.Key) {
                    case ConsoleKey.DownArrow:
                        gameField.Move(gameField.player_2, Orientation.Direction.DOWN);
                        break;
                    case ConsoleKey.UpArrow:
                        gameField.Move(gameField.player_2, Orientation.Direction.UP);
                        break;
                    case ConsoleKey.S:
                        gameField.Move(gameField.player_1, Orientation.Direction.DOWN);
                        break;
                    case ConsoleKey.W:
                        gameField.Move(gameField.player_1, Orientation.Direction.UP);
                        break;
                    default:
                        break;
                }
            }
        }

        static void PrintGameField() {
            while (true) {
                //Thread.Sleep(100);
                gameField.DrawField();
            }
        }
    }
}
