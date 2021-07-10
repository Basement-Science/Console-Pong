using System;
using System.Threading;
using System.Threading.Tasks;
using SharpDX.DirectInput;

namespace ConsolePong {
    class Program {
        private static GameField gameField = new(25, 120);
        private static Keyboard keyboard = new Keyboard(new DirectInput());

        static async Task Main(string[] args) {
            var inputHandler = new Task(ReadKeys);
            var outputHandler = new Task(PrintGameField);
            var ballHandler = new Task(ballTicker);

            Console.CancelKeyPress += new ConsoleCancelEventHandler(ExitHandler);
            Console.CursorVisible = false;

            keyboard.Properties.BufferSize = 128;
            keyboard.Acquire();

            inputHandler.Start();
            outputHandler.Start();
            ballHandler.Start();

            await inputHandler;
            await outputHandler;
            await ballHandler;
        }

        private static void ExitHandler(object sender, ConsoleCancelEventArgs e) {
            e.Cancel = true;
        }

        class ActionSpammer {
            public Key assignedKey { get; private set; }
            private Task task;
            private Action action;

            private volatile bool run = false;

            private void spam() {
                while (run) {
                    action.Invoke();
                    Thread.Sleep(50);
                }
            }

            public void startSpam() {
                if (task.Status == TaskStatus.Created) {
                    run = true;
                    task.Start();
                } else {
                    throw new ThreadInterruptedException("something wrong with spammer for " + assignedKey);
                }
            }

            public async void stopSpam() {
                run = false;
                await task;
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
                gameField.Move(gameField.player_1, Orientation.Direction.UP);
            });
            Action action_S = new Action(() => {
                gameField.Move(gameField.player_1, Orientation.Direction.DOWN);
            });
            Action action_Up = new Action(() => {
                gameField.Move(gameField.player_2, Orientation.Direction.UP);
            });
            Action action_Down = new Action(() => {
                gameField.Move(gameField.player_2, Orientation.Direction.DOWN);
            });

            ActionSpammer spammer_W = new ActionSpammer(Key.W, action_W);
            ActionSpammer spammer_S = new ActionSpammer(Key.S, action_S);
            ActionSpammer spammer_Up = new ActionSpammer(Key.Up, action_Up);
            ActionSpammer spammer_Down = new ActionSpammer(Key.Down, action_Down);

            while (true) {
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
            return;
        }

        static void PrintGameField() {
            while (true) {
                Thread.Sleep(10);
                gameField.DrawField();
            }
        }

        private static void ballTicker() {
            while (true) {
                Thread.Sleep(10);
                gameField.ProcessBall();
            }
        }
    }
}
