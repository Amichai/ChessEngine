using ChessKit.ChessLogic;

namespace ChessGameBrowser.Web.Models
{
    public sealed class InaccurateMoveModel
    {
        private readonly Position _p;

        public InaccurateMoveModel(Position p, string played, TargetMoves target)
        {
            _p = p;
            Played = played;
            Target = target;
        }

        public string Position => Fen.Print(_p);

        public string Played { get; }

        public TargetMoves Target { get; }
    }
}