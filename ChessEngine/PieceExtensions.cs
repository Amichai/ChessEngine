using System;
using System.Collections.Generic;

namespace ChessEngine {
    public static class PieceExtensions {
        public static List<CellCoordinate> GetAvailableCells(int col, int row, Piece piece, BoardState board, MoveList moves) {
            List<CellCoordinate> toReturn = new List<CellCoordinate>();
            switch (piece.PieceType) {
                case PieceType.Rook:
                    orthogonal(toReturn, col, row, piece, board);
                    break;
                case PieceType.Pawn:
                    pawn(toReturn, col, row, piece, board, moves.Last, piece.Color == SideColor.White);
                    break;
                case PieceType.Bishop:
                    diagonal(toReturn, col, row, piece, board);
                    break;
                case PieceType.King:
                    adjacent(toReturn, col, row, piece.Color, board);
                    if (moves.CanCastleRight(piece.Color) && board.Get(col + 1, row) == null && board.Get(col + 2, row) == null) {
                        addCell(toReturn, col + 2, row, piece.Color, board);
                    }
                    if (moves.CanCastleLeft(piece.Color) && board.Get(col - 1, row) == null && board.Get(col - 2, row) == null) {
                        addCell(toReturn, col - 2, row, piece.Color, board);
                    }
                    break;
                case PieceType.Knight:
                    addCell(toReturn, col + 2, row + 1, piece.Color, board);
                    addCell(toReturn, col + 1, row + 2, piece.Color, board);
                    addCell(toReturn, col + 2, row - 1, piece.Color, board);
                    addCell(toReturn, col - 1, row + 2, piece.Color, board);
                    addCell(toReturn, col - 2, row - 1, piece.Color, board);
                    addCell(toReturn, col - 1, row - 2, piece.Color, board);
                    addCell(toReturn, col - 2, row + 1, piece.Color, board);
                    addCell(toReturn, col + 1, row - 2, piece.Color, board);
                    break;
                case PieceType.Queen:
                    diagonal(toReturn, col, row, piece, board);
                    orthogonal(toReturn, col, row, piece, board);
                    break;
                default:
                    throw new Exception();
            }

            return toReturn;
        }


        /// <summary>Ignores other pieces on the board. Returns all avaiable cells if the board was empty.</summary>
        public static List<CellCoordinate> GetAvailableCells(this Cell cell, BoardState board, MoveList moves) {
            var col = cell.Col;
            var row = cell.Row;
            return PieceExtensions.GetAvailableCells(col, row, cell.Piece, board, moves);
        }

        private static void adjacent(List<CellCoordinate> toReturn, int col, int row, SideColor color, BoardState board) {
            addCell(toReturn, col + 1, row, color, board);
            addCell(toReturn, col + 1, row + 1, color, board);
            addCell(toReturn, col + 1, row - 1, color, board);
            addCell(toReturn, col - 1, row + 1, color, board);
            addCell(toReturn, col - 1, row - 1, color, board);
            addCell(toReturn, col, row + 1, color, board);
            addCell(toReturn, col, row - 1, color, board);
            addCell(toReturn, col - 1, row, color, board);
        }

        private static void diagonal(List<CellCoordinate> toReturn, int col, int row, Piece piece, BoardState board) {
            int c, r;
            for (int i = 1; i < 8; i++) {
                c = col + i;
                r = row + i;
                if (c > 8 || r > 8) { break; }
                if (board.Get(c, r) != null) {
                    if (board.Get(c, r).Color != piece.Color) {
                        addCell(toReturn, c, r);
                    }
                    break;
                }
                addCell(toReturn, c, r);
            }
            for (int i = 1; i < 8; i++) {
                c = col - i;
                r = row - i;
                if (r < 1 || c < 1) {
                    break;
                }
                if (board.Get(c, r) != null) {
                    if (board.Get(c, r).Color != piece.Color) {
                        addCell(toReturn, c, r);
                    }
                    break;
                }
                addCell(toReturn, c, r);
            }
            for (int i = 1; i < 8; i++) {
                c = col + i;
                r = row - i;
                if (c > 8 || r < 1) {
                    break;
                }
                if (board.Get(c, r) != null) {
                    if (board.Get(c, r).Color != piece.Color) {
                        addCell(toReturn, c, r);
                    }
                    break;
                }
                addCell(toReturn, c, r);
            }
            for (int i = 1; i < 8; i++) {
                c = col - i;
                r = row + i;
                if (c < 1 || r > 8) { break; }
                if (board.Get(c, r) != null) {
                    if (board.Get(c, r).Color != piece.Color) {
                        addCell(toReturn, c, r);
                    }
                    break;
                }
                addCell(toReturn, c, r);
            }
        }

        private static void addCell(List<CellCoordinate> toReturn, int c, int r, SideColor color, BoardState board)
        {
            if (isOnTheBoard(c, r))
            {
                if (board.Get(c, r) == null || board.Get(c, r).Color != color)
                {
                    toReturn.Add(new CellCoordinate(c, r));
                }
            }
        }

        private static bool isOnTheBoard(int c, int r)
        {
            return c < 9 && r < 9 && c > 0 && r > 0;
        }

        private static void addCell(List<CellCoordinate> toReturn, int c, int r) {
            if (isOnTheBoard(c, r)) {
                toReturn.Add(new CellCoordinate(c, r));
            }
        }

        private static void pawn(List<CellCoordinate> toReturn, int col, int row, Piece piece, BoardState board, SingleMove lastMove, bool facingUpward) {
            int c, r;
            int maxSteps = 1;
            if ((facingUpward && row == 2) || (!facingUpward && row == 7)) {
                maxSteps = 2;
            }
            for (int i = 1; i <= maxSteps; i++) {
                c = col;
                if (facingUpward) {
                    r = row + i;
                } else {
                    r = row - i;
                }
                if (board.Get(c, r) == null) {
                    addCell(toReturn, c, r);
                } else {
                    break;
                }
            }
            c = col;
            r = row;
            var c1 = board.GetPiece(c + 1, r + 1);
            var c2 = board.GetPiece(c - 1, r - 1);
            var c3 = board.GetPiece(c + 1, r - 1);
            var c4 = board.GetPiece(c - 1, r + 1);

            if (facingUpward && c1 != null && c1.Color != piece.Color) {
                addCell(toReturn, c + 1, r + 1);
            }
            if (facingUpward && c4 != null && c4.Color != piece.Color) {
                addCell(toReturn, c - 1, r + 1);
            }


            if (lastMove != null)
            {
                var epTarget = lastMove.EnPassantTarget;
                if (epTarget != null)
                {
                    if (facingUpward && epTarget.Col == c + 1 && epTarget.Row == r + 1)
                    {
                        addCell(toReturn, c + 1, r + 1);
                    }
                    if (facingUpward && epTarget.Col == c - 1 && epTarget.Row == r + 1)
                    {
                        addCell(toReturn, c - 1, r + 1);
                    }

                    if (!facingUpward && epTarget.Col == c + 1 && epTarget.Row == r - 1)
                    {
                        addCell(toReturn, c + 1, r - 1);
                    }
                    if (!facingUpward && epTarget.Col == c - 1 && epTarget.Row == r - 1)
                    {
                        addCell(toReturn, c - 1, r - 1);
                    }
                }
            }

            if (!facingUpward && c2 != null && c2.Color != piece.Color) {
                addCell(toReturn, c - 1, r - 1);
            }
            if (!facingUpward && c3 != null && c3.Color != piece.Color) {
                addCell(toReturn, c + 1, r - 1);
            }
        }

        public static Piece GetPiece(this BoardState board, CellCoordinate c) {
            return board.GetPiece(c.col, c.row);
        }

        public static Piece GetPiece(this BoardState board, int i, int j) {
            if (i < 1 || i > 7 || j < 1 || j > 7) {
                return null;
            } else {
                return board.Get(i, j);
            }
        }

        private static void orthogonal(List<CellCoordinate> toReturn, int col, int row, Piece piece, BoardState board) {
            int c, r;
            for (int i = 1; i < 8; i++) {
                c = col;
                r = row + i;
                if (r > 7) {
                    break;
                }
                if (board.Get(c, r) != null) {
                    if (board.Get(c, r).Color != piece.Color) {
                        addCell(toReturn, c, r);
                    }
                    break;
                }
                addCell(toReturn, c, r);
            }

            for (int i = 1; i < 8; i++) {
                c = col;
                r = row - i;
                if (r < 1) {
                    break;
                }
                if (board.Get(c, r) != null) {
                    if (board.Get(c, r).Color != piece.Color) {
                        addCell(toReturn, c, r);
                    }
                    break;
                }
                addCell(toReturn, c, r);
            }

            for (int i = 1; i < 8; i++) {
                c = col + i;
                r = row;
                if (c > 7) {
                    break;
                }
                if (board.Get(c, r) != null) {
                    if (board.Get(c, r).Color != piece.Color) {
                        addCell(toReturn, c, r);
                    }
                    break;
                }
                addCell(toReturn, c, r);
            }

            for (int i = 1; i < 8; i++) {
                c = col - i;
                r = row;
                if (c < 1) {
                    break;
                }
                if (board.Get(c, r) != null) {
                    if (board.Get(c, r).Color != piece.Color) {
                        addCell(toReturn, c, r);
                    }
                    break;
                }
                addCell(toReturn, c, r);
            }
        }

        public static int GetCellID(int col, int row) {
            return col * 8 + row;
        }

        public static string ToShortString(this PieceType p) {
            switch (p) {
                case PieceType.Queen:
                    return "Q";
                case PieceType.Bishop:
                    return "B";
                case PieceType.King:
                    return "K";
                case PieceType.Knight:
                    return "N";
                case PieceType.Pawn:
                    return "P";
                case PieceType.Rook:
                    return "R";
                default:
                    throw new NotImplementedException();
            }
        }

        public static BoardMove ApplyMove(this BoardState board, int startCol, int startRow, CellCoordinate dest, Piece p) {
            var clone = board.Clone();

            if (p.PieceType == PieceType.Pawn && startCol != dest.col && clone.Get(dest.col, dest.row) == null) {
                if (p.Color == SideColor.Black) {
                    clone.Remove(dest.col, dest.row - 1);
                } else {
                    clone.Remove(dest.col, dest.row + 1);
                }
            }

            clone.Remove(startCol, startRow);
            var taken = clone.Get(dest.col, dest.row);
            clone.Set(dest.col, dest.row, p);
            var start = new CellCoordinate(startCol, startRow);
            var move = new SingleMove(p.PieceType, start, dest, p.Color, taken);
            var toReturn = new BoardMove() {
                Board = clone,
                Move = move
            };

            var colDiff = dest.col - startCol;
            if (p.PieceType == PieceType.King && Math.Abs(colDiff) == 2) {
                if (colDiff == 2) {
                    clone.Move(7, startRow, startCol + 1, startRow);
                } else {
                    clone.Move(0, startRow, startCol - 1, startRow);
                }
            }

            return toReturn;
        }
    }
}
