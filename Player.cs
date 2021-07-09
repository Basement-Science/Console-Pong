namespace ConsolePong {
    internal class Player {
        public int xCenter { get; private set; }
        public int length { get; private set; }
        public int xTop { get; private set; }
        public int xBottom { get; private set; }

        public Player(int startPos, int length = 6) {
            this.xCenter = startPos;
            this.length = length;
            this.xTop = xCenter + Orientation.getNext(Orientation.Direction.UP, this.length / 2);
            this.xBottom = xTop + Orientation.getNext(Orientation.Direction.DOWN, this.length);
        }

        public void Move(Orientation.Direction direction, int amount = 1) {
            int tmp = Orientation.getNext(direction, amount);
            xCenter += tmp;
            xTop += tmp;
            xBottom += tmp;
        }
    }
}