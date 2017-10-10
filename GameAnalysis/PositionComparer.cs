using ChessKit.ChessLogic;
using ChessKit.ChessLogic.Primitives;

namespace GameAnalysis
{
    internal sealed class PiecePositionComparer
    {
        public static PiecePositionCompareResult Compare(TraversedPosition newPosition, BoardPiece piece)
        {
            ///Did the piece move, stay same, or get captured?
            var newPieces = newPosition.Position.GetPieces();
            foreach (var p in newPieces)
            {
                if (piece.Piece == p.Piece && piece.X == p.X && p.Y == piece.Y)
                {
                    return PiecePositionCompareResult.Survived;
                }
            }

            var newPiecePosition = piece;

            foreach (var m in newPosition.Moves.Moves)
            {
                var move = m.Move;
                var x = move.FromCell.GetX();
                var y = move.FromCell.GetY();

                var destX = move.ToCell.GetX();
                var destY = move.ToCell.GetY();

                var newPieceColor = newPiecePosition.Piece.Color();

                var activeColor = m.Position.ActiveColor();
                var colorsMatch = (newPieceColor == Color.White && activeColor == "w") ||
                                  (newPieceColor == Color.Black && activeColor == "b");

                if (x == newPiecePosition.X && y == newPiecePosition.Y && m.Piece == newPiecePosition.Piece.PieceType() && colorsMatch)
                {
                    newPiecePosition = new BoardPiece(piece.Piece, destX, destY);
                }


                if (!colorsMatch && destX == newPiecePosition.X &&
                    destY == newPiecePosition.Y)
                {
                    if (newPiecePosition == piece)
                    {
                        return PiecePositionCompareResult.CapturedInPlace();
                    }
                    else
                    {
                        return PiecePositionCompareResult.CapturedInAction();
                    }
                }
            }

            return PiecePositionCompareResult.Moved(newPiecePosition);
        }
    }

    internal sealed class PiecePositionCompareResult
    {
        public string CompareResult { get; }

        public BoardPiece Piece { get; }

        public static string SurvivedResultValue = "Survived";
        public static string MovedResultValue = "Moved";
        public static string CapturedInActionResultValue = "CapturedInAction";
        public static string CapturedInPlaceResultValue = "CapturedInPlace";

        private PiecePositionCompareResult(string result, BoardPiece piece = null)
        {
            CompareResult = result;
            Piece = piece;
        }

        public static PiecePositionCompareResult Survived = new PiecePositionCompareResult(SurvivedResultValue);

        public static PiecePositionCompareResult Moved(BoardPiece piece)
        {
            return new PiecePositionCompareResult(MovedResultValue, piece);
        }

        public static PiecePositionCompareResult CapturedInAction()
        {
            return new PiecePositionCompareResult(CapturedInActionResultValue);
        }

        public static PiecePositionCompareResult CapturedInPlace()
        {
            return new PiecePositionCompareResult(CapturedInPlaceResultValue);
        }
    }
}