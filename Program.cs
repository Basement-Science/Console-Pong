using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsolePong {
    class Program {
        static async Task Main(string[] args) {
            _ = Task.Run(() => {
                while (true) {
                    Console.ReadKey(true);
                    Console.WriteLine("Andere ausgabe!");
                }
            });

            await Task.Run(() => {
                while (true) {
                    Console.WriteLine("Hello World!");
                    Thread.Sleep(100);
                }

            });
        }
    }
}
