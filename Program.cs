using System;
using System.Threading;
using System.Threading.Tasks;
using SharpDX.DirectInput;
using ManyMouseSharp;
using Pastel;
using System.Drawing;

namespace ConsolePong {
    class Program {
        private static GameField gameField = new(25, 120);
        private static Keyboard keyboard = new Keyboard(new DirectInput());

        // interpret uint.MaxValue as invalid
        private const uint mouseNotInitialized = uint.MaxValue;
        private static uint mouse_L = mouseNotInitialized, mouse_R = mouseNotInitialized;

        // for Thread communication
        private static volatile bool keepRunning = true;
        private static volatile bool miceInitialized = false;
        private static volatile bool EscPressed = false;
        private static Key? lastKey = null;

        private readonly string LoremIpsum = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.";

        static async Task Main(string[] args) {
            var keyboardHandler = new Task(ReadKeys);
            var mouseHandler = new Task(ProcessMice);
            var outputHandler = new Task(PrintGameField);
            var ballHandler = new Task(ballTicker);

            Console.CancelKeyPress += new ConsoleCancelEventHandler(ExitHandler);
            Console.CursorVisible = false;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            keyboard.Properties.BufferSize = 128;
            keyboard.Acquire();
            keyboardHandler.Start();

            mouseHandler.Start();
            outputHandler.Start();
            while (!miceInitialized) {
                Thread.Sleep(10);
            }
            Thread.Sleep(750);
            gameField.textArea.ClearField();
            ballHandler.Start();

            await keyboardHandler;
            await mouseHandler;
            await outputHandler;
            await ballHandler;
        }

        private static void ProcessMice() {
            assignMice();
            gameField.textArea.WriteToField("--> Starting game...");
            miceInitialized = true;
            int accu_L = 0, accu_R = 0;
            while (keepRunning) {
                ManyMouseEvent mouseEvent;
                while (ManyMouse.PollEvent(out mouseEvent) > 0) {
                    switch (mouseEvent.type) {
                        case ManyMouseEventType.MANYMOUSE_EVENT_ABSMOTION:
                            // not implemented, ignore it
                            break;
                        case ManyMouseEventType.MANYMOUSE_EVENT_RELMOTION:
                            if (mouseEvent.item == 1) {
                                // the only real case, movement in Y direction
                                Misc.Direction direction = mouseEvent.value > 0 
                                    ? Misc.Direction.DOWN : Misc.Direction.UP;

                                const int sensitivity = 16;
                                if (mouseEvent.device == mouse_L) {
                                    accu_L += mouseEvent.value;
                                    //Console.WriteLine(accu_L);
                                    //printMouseEvent(mouseEvent);
                                    while (Math.Abs(accu_L) > sensitivity) { 
                                        gameField.Move(gameField.player_1, direction);
                                        accu_L -= accu_L > 0 ? sensitivity : accu_L < 0 ? -sensitivity : 0;
                                    }
                                }
                                if (mouseEvent.device == mouse_R) {
                                    accu_R += mouseEvent.value;
                                    //Console.WriteLine(accu_R);
                                    //printMouseEvent(mouseEvent);
                                    while (Math.Abs(accu_R) > sensitivity) {
                                        gameField.Move(gameField.player_2, direction);
                                        accu_R -= accu_R > 0 ? sensitivity : accu_R < 0 ? -sensitivity : 0;
                                    }
                                }
                            } 
                            break;
                        case ManyMouseEventType.MANYMOUSE_EVENT_DISCONNECT:
                            throw new NotImplementedException("Disconnecting a Mouse " +
                                "and reacquiring it when it was in use is not supported.");
                        case ManyMouseEventType.MANYMOUSE_EVENT_MAX:
                            break;
                        default:
                            break;
                    }
                }
            }
            // Prompts players to select their mice to be used.
            void assignMice() {
                ManyMouse.Init();
                switch (ManyMouse.AmountOfMiceDetected) {
                    case 0:
                        gameField.textArea.WriteToField("No Mouse detected. Only keyboard control will be available.");
                        break;
                    case 1:
                        gameField.textArea.WriteToField("1 Mouse detected. Please move the player that will use the keyboard. " +
                            "The other player will be able to use the Mouse. ");
                        while (lastKey == null) { Thread.Sleep(10); }
                        if (EscPressed) {
                            gameField.textArea.WriteToField("Skipping Mouse assignment");
                            EscPressed = false;
                        } else if (lastKey == Key.W || lastKey == Key.S) {
                            mouse_L = 0;
                        } else if (lastKey == Key.Up || lastKey == Key.Down) {
                            mouse_R = 0;
                        } else {
                            // irrelevant key was pressed. Try again.
                            goto case 1;
                        }
                        break;
                    default:
                        gameField.textArea.WriteToField($"Detected {ManyMouse.AmountOfMiceDetected} Mice.");
                        gameField.textArea.WriteToField($"Please CLICK a button on LEFT Player's mouse... ");
                        ManyMouseEvent mouseEvent;
                        int numAssigned = 0;
                        while (numAssigned < 2) {
                            while (ManyMouse.PollEvent(out mouseEvent) > 0) {
                                switch (mouseEvent.type) {
                                    case ManyMouseEventType.MANYMOUSE_EVENT_BUTTON:
                                        if (mouseEvent.value == 1) {
                                            // MouseButton DOWN
                                            switch (numAssigned) {
                                                case 0:
                                                    mouse_L = mouseEvent.device;
                                                    gameField.textArea.AppendToLastLine("success!"/*.Pastel(Color.Green)*/);
                                                    gameField.textArea.WriteToField($"Please CLICK a button on RIGHT Player's mouse... ");
                                                    numAssigned++;
                                                    break;
                                                case 1:
                                                    mouse_R = mouseEvent.device;
                                                    gameField.textArea.AppendToLastLine(mouse_L != mouse_R ?
                                                        "success!"/*.Pastel(Color.Green)*/ :
                                                        "warning: selected the same device twice."
                                                        /*.Pastel(Color.Yellow)*/);
                                                    numAssigned++;
                                                    break;
                                                default:
                                                    return;
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            if (EscPressed) {
                                gameField.textArea.AppendToLastLine("skipping.");
                                numAssigned++;
                                if (numAssigned == 1) {
                                    gameField.textArea.WriteToField($"Please CLICK a button on RIGHT Player's mouse... ");
                                }
                                EscPressed = false;
                            }
                        }
                        break;
                }
            }
            void printMouseEvent(ManyMouseEvent mouseEvent) {
                Console.WriteLine(
                        $"device: {mouseEvent.device}\n" +
                        $"item: {mouseEvent.item}\n" +
                        $"type: {mouseEvent.type}\n" +
                        $"value: {mouseEvent.value}\n" +
                        $"maxval: {mouseEvent.maxval}\n" +
                        $"minval: {mouseEvent.minval}\n");
            }
        }

        protected static void ExitHandler(object sender, ConsoleCancelEventArgs e) {
            keepRunning = false;
            e.Cancel = true;
        }

        private class ActionSpammer {
            public Key assignedKey { get; private set; }
            private Task task;
            private KeyPressedAction action;
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

            public ActionSpammer(Key key, KeyPressedAction action) {
                assignedKey = key;
                this.action = action;
                task = new Task(spam);
            }
        }

        private delegate void KeyPressedAction();
        static void ReadKeys() {
            KeyPressedAction action_W =
                () => gameField.Move(gameField.player_1, Misc.Direction.UP);
            KeyPressedAction action_S =
                () => gameField.Move(gameField.player_1, Misc.Direction.DOWN);
            KeyPressedAction action_Up =
                () => gameField.Move(gameField.player_2, Misc.Direction.UP);
            KeyPressedAction action_Down =
                () => gameField.Move(gameField.player_2, Misc.Direction.DOWN);

            ActionSpammer spammer_W = new ActionSpammer(Key.W, action_W);
            ActionSpammer spammer_S = new ActionSpammer(Key.S, action_S);
            ActionSpammer spammer_Up = new ActionSpammer(Key.Up, action_Up);
            ActionSpammer spammer_Down = new ActionSpammer(Key.Down, action_Down);

            while (keepRunning) {
                keyboard.Poll();
                var buffer = keyboard.GetBufferedData();
                foreach (KeyboardUpdate update in buffer) {
                    lastKey = update.Key;
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
                        case Key.Escape:
                            if (update.IsPressed) {
                                EscPressed = true;
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
            gameField.ClearScreen();
        }

        private static void ballTicker() {
            while (keepRunning) {
                Thread.Sleep(4);
                gameField.ProcessBalls();
            }
        }
    }
}
