using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine {
    public static class InitialBoardConfigurationFactory {
        public static Piece Get(int id) {
            switch (id) {
                case 0:
                    return new Piece(SideColor.Black, PieceType.Rook);
                case 1:
                    return new Piece(SideColor.Black, PieceType.Pawn);
                case 6:
                    return new Piece(SideColor.White, PieceType.Pawn);
                case 7:
                    return new Piece(SideColor.White, PieceType.Rook);

                case 8:
                    return new Piece(SideColor.Black, PieceType.Knight);
                case 9:
                    return new Piece(SideColor.Black, PieceType.Pawn);
                case 14:
                    return new Piece(SideColor.White, PieceType.Pawn);
                case 15:
                    return new Piece(SideColor.White, PieceType.Knight);

                case 16:
                    return new Piece(SideColor.Black, PieceType.Bishop);
                case 17:
                    return new Piece(SideColor.Black, PieceType.Pawn);
                case 22:
                    return new Piece(SideColor.White, PieceType.Pawn);
                case 23:
                    return new Piece(SideColor.White, PieceType.Bishop);


                case 24:
                    return new Piece(SideColor.Black, PieceType.Queen);
                case 25:
                    return new Piece(SideColor.Black, PieceType.Pawn);
                case 30:
                    return new Piece(SideColor.White, PieceType.Pawn);
                case 31:
                    return new Piece(SideColor.White, PieceType.Queen);


                case 32:
                    return new Piece(SideColor.Black, PieceType.King);
                case 33:
                    return new Piece(SideColor.Black, PieceType.Pawn);
                case 38:
                    return new Piece(SideColor.White, PieceType.Pawn);
                case 39:
                    return new Piece(SideColor.White, PieceType.King);


                case 40:
                    return new Piece(SideColor.Black, PieceType.Bishop);
                case 41:
                    return new Piece(SideColor.Black, PieceType.Pawn);
                case 46:
                    return new Piece(SideColor.White, PieceType.Pawn);
                case 47:
                    return new Piece(SideColor.White, PieceType.Bishop);

                case 48:
                    return new Piece(SideColor.Black, PieceType.Knight);
                case 49:
                    return new Piece(SideColor.Black, PieceType.Pawn);
                case 54:
                    return new Piece(SideColor.White, PieceType.Pawn);
                case 55:
                    return new Piece(SideColor.White, PieceType.Knight);


                case 56:
                    return new Piece(SideColor.Black, PieceType.Rook);
                case 57:
                    return new Piece(SideColor.Black, PieceType.Pawn);
                case 62:
                    return new Piece(SideColor.White, PieceType.Pawn);
                case 63:
                    return new Piece(SideColor.White, PieceType.Rook);
                default:
                    return null;
            }
        }
    }
}
