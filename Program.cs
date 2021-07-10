using System;
using System.Threading;
using System.Threading.Tasks;
using SharpDX.DirectInput;

namespace ConsolePong {
    class Program {
        private static GameField gameField = new(25, 120);
        private static Keyboard keyboard = new Keyboard(new DirectInput());

        private static volatile bool keepRunning = true;

        static async Task Main(string[] args) {
            var inputHandler = new Task(ReadKeys);
            var outputHandler = new Task(PrintGameField);
            var ballHandler = new Task(ballTicker);

            Console.CancelKeyPress += new ConsoleCancelEventHandler(ExitHandler);
            Console.CursorVisible = false;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            keyboard.Properties.BufferSize = 128;
            keyboard.Acquire();

            inputHandler.Start();
            outputHandler.Start();
            ballHandler.Start();

            await inputHandler;
            await outputHandler;
            await ballHandler;
        }

        protected static void ExitHandler(object sender, ConsoleCancelEventArgs e) {
            keepRunning = false;
            e.Cancel = true;
        }

        class ActionSpammer {
            public Key assignedKey { get; private set; }
            private Task task;
            private Action action;
            private CancellationTokenSource cts;

            private volatile bool run = false;

            private void spam() {
                while (run) {
                    action.Invoke();
                    Task.WaitAny(Task.Delay(50, cts.Token));
                }
            }

            public void startSpam() {
                if (task.Status == TaskStatus.Created) {
                    // normal situation
                    run = true;
                    cts = new();
                    task.Start();
                } else {
                    throw new ThreadInterruptedException($"something wrong with spammer for {assignedKey} key.");
                }
            }

            public void stopSpam() {
                run = false;
                cts.Cancel();
                task.Wait();
                task.Dispose();
                task = new Task(spam);
            }

            public ActionSpammer(Key key, Action action) {
                assignedKey = key;
                this.action = action;
                task = new Task(spam);
            }
        }

        static void ReadKeys() {
            Action action_W = new Action(() => {
                gameField.Move(gameField.player_1, Misc.Direction.UP);
            });
            Action action_S = new Action(() => {
                gameField.Move(gameField.player_1, Misc.Direction.DOWN);
            });
            Action action_Up = new Action(() => {
                gameField.Move(gameField.player_2, Misc.Direction.UP);
            });
            Action action_Down = new Action(() => {
                gameField.Move(gameField.player_2, Misc.Direction.DOWN);
            });

            ActionSpammer spammer_W = new ActionSpammer(Key.W, action_W);
            ActionSpammer spammer_S = new ActionSpammer(Key.S, action_S);
            ActionSpammer spammer_Up = new ActionSpammer(Key.Up, action_Up);
            ActionSpammer spammer_Down = new ActionSpammer(Key.Down, action_Down);

            while (keepRunning) {
                keyboard.Poll();
                var buffer = keyboard.GetBufferedData();
                foreach (KeyboardUpdate update in buffer) {
                    
                    switch (update.Key) {
                        case Key.W:
                            if (update.IsPressed) {
                                spammer_W.startSpam();
                            } else if (update.IsReleased) {
                                spammer_W.stopSpam();
                            }
                            break;
                        case Key.S:
                            if (update.IsPressed) {
                                spammer_S.startSpam();
                            } else if (update.IsReleased) {
                                spammer_S.stopSpam();
                            }
                            break;
                        case Key.Up:
                            if (update.IsPressed) {
                                spammer_Up.startSpam();
                            } else if (update.IsReleased) {
                                spammer_Up.stopSpam();
                            }
                            break;
                        case Key.Down:
                            if (update.IsPressed) {
                                spammer_Down.startSpam();
                            } else if (update.IsReleased) {
                                spammer_Down.stopSpam();
                            }
                            break;
                        default:
                            break;
                    }
                }
                Thread.Sleep(1);
            }
        }

        private static void PrintGameField() {
            while (keepRunning) {
                Thread.Sleep(4);
                gameField.DrawField();
                gameField.PrintScoreBar();
            }
        }

        private static void ballTicker() {
            while (keepRunning) {
                Thread.Sleep(4);
                gameField.ProcessBalls();
            }
        }
    }
}
