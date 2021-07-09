namespace ConsolePong {
    internal class Player {
        public int xCenter { get; private set; }
        public int length { get; private set; }
        public int xTop { get; private set; }
        public int xBottom { get; private set; }

        public Player(int v, int length = 8) {
            this.xCenter = v;
            this.length = length;
            this.xTop = xCenter - this.length / 2;
            this.xTop = xTop + this.length;
        }

        public void Move(GameField.Direction direction) {
            switch (direction) {
                case GameField.Direction.UP:
                    xCenter += 1;
                    xTop += 1;
                    xBottom += 1;
                    break;
                case GameField.Direction.DOWN:
                    xCenter -= 1;
                    xTop -= 1;
                    xBottom -= 1;
                    break;
            }
        }
    }
}