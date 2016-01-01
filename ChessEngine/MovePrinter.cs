using System;
using ChessKit.ChessLogic;

namespace ChessEngine
{
    public static class MovePrinter
    {
        public static string Print(this LegalMove move)
        {
            string toReturn = string.Empty;

            return toReturn;
        }


        public static Piece ConvertPieceType(SideColor color, ChessKit.ChessLogic.PieceType p)
        {
            if (p.IsRook)
            {
                return new Piece(color, PieceType.Rook);
            }
            if (p.IsKnight)
            {
                return new Piece(color, PieceType.Knight);
            }
            if (p.IsBishop)
            {
                return new Piece(color, PieceType.Bishop);
            }
            if (p.IsKing)
            {
                return new Piece(color, PieceType.King);
            }
            if (p.IsQueen)
            {
                return new Piece(color, PieceType.Queen);
            }
            if (p.IsPawn)
            {
                return new Piece(color, PieceType.Pawn);
            }
            throw new Exception();
        }


        //private static string PieceString(Piece p)
        //{
        //    var piece = ConvertPieceType(p.Color, p.PieceType);
        //    switch (p.)
        //    {
                    
        //    }
        //}
    }
}