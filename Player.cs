namespace ConsolePong {
    internal class Player {
        public int xCenter { get; private set; }
        public int length { get; private set; }
        public int xTop { get; private set; }
        public int xBottom { get; private set; }
        public int score { get; private set; } = 0;

        public Player(int startPos, int length = 6) {
            this.xCenter = startPos;
            this.length = length;
            this.xTop = xCenter + Misc.getNext(Misc.Direction.UP, this.length / 2);
            this.xBottom = xTop + Misc.getNext(Misc.Direction.DOWN, this.length - 1);
        }

        public void Move(Misc.Direction direction, int amount = 1) {
            int tmp = Misc.getNext(direction, amount);
            xCenter += tmp;
            xTop += tmp;
            xBottom += tmp;
        }

        public void addScore(int points = 1) {
            score += points;
        }
    }
}