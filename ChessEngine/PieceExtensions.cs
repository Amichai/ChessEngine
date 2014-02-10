using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine {
    public static class PieceExtensions {
        /// <summary>Ignores other pieces on the board. Returns all avaiable cells if the board was empty.</summary>
        public static List<CellCoordinate> GetAvailableCells(this Cell cell, Piece[][] board, SingeMove lastMove) {
            List<CellCoordinate> toReturn = new List<CellCoordinate>();
            var col = cell.col;
            var row = cell.row;
            switch (cell.PieceType) {
                case PieceType.Rook:
                    orthogonal(toReturn, cell,  board);
                    break;
                case PieceType.Pawn:
                    pawn(toReturn, cell, board, lastMove, cell.Piece.Color == SideColor.White);
                    break;
                case PieceType.Bishop:
                    diagonal(toReturn, cell, board);
                    break;
                case PieceType.King:
                    adjacent(toReturn, col, row, cell.Piece.Color, board);
                    break;
                case PieceType.Knight:
                    addCell(toReturn, col + 2, row + 1, cell.Piece.Color, board);
                    addCell(toReturn, col + 1, row + 2, cell.Piece.Color, board);
                    addCell(toReturn, col + 2, row - 1, cell.Piece.Color, board);
                    addCell(toReturn, col - 1, row + 2, cell.Piece.Color, board);
                    addCell(toReturn, col - 2, row - 1, cell.Piece.Color, board);
                    addCell(toReturn, col - 1, row - 2, cell.Piece.Color, board);
                    addCell(toReturn, col - 2, row + 1, cell.Piece.Color, board);
                    addCell(toReturn, col + 1, row - 2, cell.Piece.Color, board);
                    break;
                case PieceType.Queen:
                    diagonal(toReturn, cell, board);
                    orthogonal(toReturn, cell, board);
                    break;
                default:
                    throw new Exception();
            }

            return toReturn;
        }

        private static void adjacent(List<CellCoordinate> toReturn, int col, int row, SideColor color, Piece[][] board) {
            addCell(toReturn, col + 1, row, color, board);
            addCell(toReturn, col + 1, row + 1, color, board);
            addCell(toReturn, col + 1, row - 1, color, board);
            addCell(toReturn, col - 1, row + 1, color, board);
            addCell(toReturn, col - 1, row - 1, color, board);
            addCell(toReturn, col, row + 1, color, board);
            addCell(toReturn, col, row - 1, color, board);
            addCell(toReturn, col - 1, row, color, board);
        }

        private static void diagonal(List<CellCoordinate> toReturn, Cell cell, Piece[][] board) {
            int c, r;
            int col = cell.col;
            int row = cell.row;
            for (int i = 1; i < 8; i++) {
                c = col + i;
                r = row + i;
                if (c > 7 || r > 7) { break; }
                if (board[c][r] != null) {
                    if (board[c][r].Color != cell.Piece.Color) {
                        addCell(toReturn, c, r);
                    }
                    break;
                }
                addCell(toReturn, c, r);
            }
            for (int i = 1; i < 8; i++) {
                c = col - i;
                r = row - i;
                if (r < 0 || c < 0) {
                    break;
                }
                if (board[c][r] != null) {
                    if (board[c][r].Color != cell.Piece.Color) {
                        addCell(toReturn, c, r);
                    }
                    break;
                }
                addCell(toReturn, c, r);
            }
            for (int i = 1; i < 8; i++) {
                c = col + i;
                r = row - i;
                if (c > 7 || r < 0) {
                    break;
                }
                if (board[c][r] != null) {
                    if (board[c][r].Color != cell.Piece.Color) {
                        addCell(toReturn, c, r);
                    }
                    break;
                }
                addCell(toReturn, c, r);
            }
            for (int i = 1; i < 8; i++) {
                c = col - i;
                r = row + i;
                if (c < 0 || r > 7) { break; }
                if (board[c][r] != null) {
                    if (board[c][r].Color != cell.Piece.Color) {
                        addCell(toReturn, c, r);
                    }
                    break;
                }
                addCell(toReturn, c, r);
            }
        }

        private static void addCell(List<CellCoordinate> toReturn, int c, int r, SideColor color, Piece[][] board) {
            if (c < 8 && r < 8 && c >= 0 && r >= 0) {
                if (board[c][r] == null || board[c][r].Color != color) {
                    toReturn.Add(new CellCoordinate(c, r));
                }
            }
        }

        private static void addCell(List<CellCoordinate> toReturn, int c, int r) {
            if (c < 8 && r < 8 && c >= 0 && r >= 0) {
                toReturn.Add(new CellCoordinate(c, r));
            }
        }

        private static void pawn(List<CellCoordinate> toReturn, Cell cell, Piece[][] board, SingeMove lastMove, bool direction) {
            int col = cell.col;
            int row = cell.row;
            int c, r;
            int maxSteps = 1;
            if ((!direction && row == 1) || (direction && row == 6)) {
                maxSteps = 2;
            }
            for (int i = 1; i <= maxSteps; i++) {
                c = col;
                if (direction) {
                    r = row - i;
                } else {
                    r = row + i;
                }
                if (board[c][r] == null) {
                    addCell(toReturn, c, r);
                } else {
                    break;
                }
            }
            c = cell.col;
            r = cell.row;
            var c1 = board.GetPiece(c + 1, r + 1);
            var c2 = board.GetPiece(c - 1, r - 1);
            var c3 = board.GetPiece(c + 1, r - 1);
            var c4 = board.GetPiece(c - 1, r + 1);

            if (!direction && c1 != null && c1.Color != cell.Piece.Color) {
                addCell(toReturn, c + 1, r + 1);
            }
            if (!direction && c4 != null && c4.Color != cell.Piece.Color) {
                addCell(toReturn, c - 1, r + 1);
            }

            if (direction && c2 != null && c2.Color != cell.Piece.Color) {
                addCell(toReturn, c - 1, r - 1);
            }
            if (direction && c3 != null && c3.Color != cell.Piece.Color) {
                addCell(toReturn, c + 1, r - 1);
            }

            Piece p;
            CellCoordinate target;
            if (direction && r == 3) {
                target = new CellCoordinate(c - 1, r);
                p = board.GetPiece(target); /// CHECK THAT THIS PIECE ARRIVED HERE LAST MOVE BY DOUBLE STEP!!
                if (p != null && p.PieceType == PieceType.Pawn && lastMove.End == target) {
                    addCell(toReturn, c - 1, r - 1);
                }
               
                target = new CellCoordinate(c + 1, r);
                p = board.GetPiece(target);
                if (p != null && p.PieceType == PieceType.Pawn && lastMove.End == target) {
                    addCell(toReturn, c + 1, r - 1);
                }
            }

            if (!direction && r == 4) {
                target = new CellCoordinate(c - 1, r);
                p = board.GetPiece(target);
                if (p != null && p.PieceType == PieceType.Pawn && lastMove.End == target) {
                    addCell(toReturn, c - 1, r + 1);
                }

                target = new CellCoordinate(c + 1, r);
                p = board.GetPiece(target);
                if (p != null && p.PieceType == PieceType.Pawn && lastMove.End == target) {
                    addCell(toReturn, c + 1, r + 1);
                }
            }
        }

        public static Piece GetPiece(this Piece[][] board, CellCoordinate c) {
            return board.GetPiece(c.Col, c.Row);
        }

        public static Piece GetPiece(this Piece[][] board, int i, int j) {
            if (i < 0 || i > 7 || j < 0 || j > 7) {
                return null;
            } else {
                return board[i][j];
            }
        }

        private static void orthogonal(List<CellCoordinate> toReturn, Cell cell, Piece[][] board) {
            int col = cell.col;
            int row = cell.row;
            int c, r;
            for (int i = 1; i < 8; i++) {
                c = col;
                r = row + i;
                if (r > 7) {
                    break;
                }
                if (board[c][r] != null) {
                    if (board[c][r].Color != cell.Piece.Color) {
                        addCell(toReturn, c, r);
                    }
                    break;
                }
                addCell(toReturn, c, r);
            }

            for (int i = 1; i < 8; i++) {
                c = col;
                r = row - i;
                if (r < 0) {
                    break;
                }
                if (board[c][r] != null) {
                    if (board[c][r].Color != cell.Piece.Color) {
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
                if (board[c][r] != null) {
                    if (board[c][r].Color != cell.Piece.Color) {
                        addCell(toReturn, c, r);
                    }
                    break;
                }
                addCell(toReturn, c, r);
            }

            for (int i = 1; i < 8; i++) {
                c = col - i;
                r = row;
                if (c < 0) {
                    break;
                }
                if (board[c][r] != null) {
                    if (board[c][r].Color != cell.Piece.Color) {
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
    }
}
