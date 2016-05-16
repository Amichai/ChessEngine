using System;
using System.Net.NetworkInformation;

namespace BoardStateFormatter
{
    internal static class PieceExtensions
    {
        public static bool IsWhite(this Piece piece)
        {
            switch (piece)
            {
                case Piece.WhitePawn:
                case Piece.WhiteKnight:
                case Piece.WhiteBishop:
                case Piece.WhiteRook:
                case Piece.WhiteKing:
                case Piece.WhiteQueen:
                    return true;
                case Piece.BlackPawn:
                case Piece.BlackKnight:
                case Piece.BlackBishop:
                case Piece.BlackRook:
                case Piece.BlackKing:
                case Piece.BlackQueen:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(piece), piece, null);
            }
        }

        public static bool IsBlack(this Piece piece)
        {
            return !piece.IsWhite();
        }

        public static bool IsSameColor(this Piece piece, Piece other)
        {
            var firstColor = piece.IsWhite();
            var secondColor = other.IsWhite();
            return firstColor == secondColor;
        }

        public static bool IsUnmovedPawn(this CellPieceModel cell)
        {
            var piece = cell.Piece;
            if (piece == null)
            {
                return false;
            }

            var p = piece.Value;

            if (p != Piece.BlackPawn && p != Piece.WhitePawn)
            {
                return false;
            }

            if (p.IsBlack() && cell.Cell.Y == 1)
            {
                return true;
            }

            if (p.IsWhite() && cell.Cell.Y == 6)
            {
                return true;
            }

            return false;
        }

    }
}