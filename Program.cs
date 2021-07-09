using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsolePong {
    class Program {
        private static GameField gameTable = new(32, 64);
        static async Task Main(string[] args) {
            var inputHandler = new Task(ReadKeys);
            var outputHandler = new Task(PrintGameField);

            Console.CancelKeyPress += new ConsoleCancelEventHandler(ExitHandler);

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
                        Console.WriteLine("downArrow");
                        gameTable.Move(gameTable.player_2, GameField.Direction.DOWN);
                        break;
                    case ConsoleKey.UpArrow:
                        Console.WriteLine("upArrow");
                        gameTable.Move(gameTable.player_2, GameField.Direction.UP);
                        break;
                    case ConsoleKey.S:
                        Console.WriteLine("s");
                        gameTable.Move(gameTable.player_1, GameField.Direction.DOWN);
                        break;
                    case ConsoleKey.W:
                        Console.WriteLine("w");
                        gameTable.Move(gameTable.player_1, GameField.Direction.UP);
                        break;
                    default:
                        break;
                }
            }
        }

        static void PrintGameField() {
            while (true) {
                Console.WriteLine("Hello World!");
                Thread.Sleep(100);
            }
        }
    }
}
