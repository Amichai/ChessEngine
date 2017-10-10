using System.Linq;
using ChessKit.ChessLogic;
using ChessKit.ChessLogic.Primitives;

namespace GameAnalysis
{
    internal sealed class MoveAndPosition
    {
        public Move Move { get; }

        public Position Position { get; }

        public PieceType Piece
        {
            get { return Position.GetPieces().Single(i => i.X == Move.FromCell.GetX() && i.Y == Move.FromCell.GetX()).Piece.PieceType(); }
        }

        public MoveAndPosition(Move move, Position pos)
        {
            Move = move;
            Position = pos;
        }
    }
}