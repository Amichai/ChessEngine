using ChessKit.ChessLogic.Primitives;

namespace GameAnalysis
{
    internal sealed class BoardPiece
    {
        public Piece Piece { get; }
        public int X { get; }
        public int Y { get; }

        public BoardPiece(Piece piece, int x, int y)
        {
            Piece = piece;
            X = x;
            Y = y;
        }
    }
}