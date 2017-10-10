using ChessKit.ChessLogic;

namespace ChessGameBrowser.Web.Models
{
    public sealed class TargetMove
    {
        public TargetMove(PositionCore position, int white, int black, int draw, string move, string san)
        {
            Position = position;

            White = white;
            Draw = draw;
            Black = black;
            Move = move;
            San = san;
        }

        public int White { get; }
        public int Draw { get; }
        public int Black { get; }
        public string Move { get; }
        public string San { get; }
        public PositionCore Position { get; }
    }
}