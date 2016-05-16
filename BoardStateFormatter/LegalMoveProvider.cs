using System;
using System.Collections.Generic;
using System.Threading;

namespace BoardStateFormatter
{
    internal static class LegalMoveProvider
    {
        private static List<MoveModel> PawnCaptures(PositionModel position, CellPieceModel pawnCell)
        {
            var moves = new List<MoveModel>();
            var cell = pawnCell.Cell;
            if (pawnCell.Piece.Value.IsWhite())
            {

                CellModel targetCell = null;
                if (cell.X - 1 >= 0)
                {
                    targetCell = new CellModel(cell.X - 1, cell.Y - 1);
                    capture(position, pawnCell, targetCell, moves);

                }

                if (cell.X + 1 < 8)
                {
                    targetCell = new CellModel(cell.X + 1, cell.Y - 1);
                    capture(position, pawnCell, targetCell, moves);

                }
            }
            else
            {
                CellModel targetCell = null;
                if (cell.X - 1 >= 0)
                {
                    targetCell = new CellModel(cell.X - 1, cell.Y + 1);
                    capture(position, pawnCell, targetCell, moves);
                }

                if (cell.X + 1 < 8)
                {
                    targetCell = new CellModel(cell.X + 1, cell.Y + 1);
                    capture(position, pawnCell, targetCell, moves);
                }
            }

            return moves;
        }

        private static void capture(PositionModel position, CellPieceModel pawnCell, CellModel targetCell, List<MoveModel> moves)
        {
            var t1 = position.Get(targetCell);
            if (t1.HasValue && !t1.Value.IsSameColor(pawnCell.Piece.Value))
            {
                moves.Add(new MoveModel()
                {
                    Added = new List<CellPieceModel> {new CellPieceModel(targetCell.Idx, pawnCell.Piece.Value)},
                    Removed = new List<CellPieceModel> {pawnCell.Clone()}
                });
            }
        }

        public static List<MoveModel>  LegalMoves(PositionModel position)
        {
            var allMoves = new List<MoveModel>();

            var cells = position.PieceCells();
            foreach (var cell in cells)
            {
                var movers = new List<Mover>();

                if (cell.Piece == null)
                {
                    continue;
                }

                if (position.IsWhitesTurnToMove && cell.Piece.Value.IsBlack())
                {
                    continue;
                }

                if (!position.IsWhitesTurnToMove && cell.Piece.Value.IsWhite())
                {
                    continue;
                }
                var canCature = cell.Piece.Value != Piece.WhitePawn && cell.Piece.Value != Piece.BlackPawn;
                switch (cell.Piece)
                {
                    case Piece.BlackPawn:
                        if (cell.IsUnmovedPawn())
                        {
                            movers.Add(Mover.OneTime(0, 2));
                        }

                        movers.Add(Mover.OneTime(0, 1));

                        allMoves.AddRange(PawnCaptures(position, cell));

                        break;
                    case Piece.WhitePawn:
                        if (cell.IsUnmovedPawn())
                        {
                            movers.Add(Mover.OneTime(0, -2));
                        }

                        movers.Add(Mover.OneTime(0, -1));

                        allMoves.AddRange(PawnCaptures(position, cell));

                        break;
                    case Piece.WhiteKnight:
                    case Piece.BlackKnight:
                        movers.Add(Mover.OneTime(2, 1));
                        movers.Add(Mover.OneTime(2, -1));
                        movers.Add(Mover.OneTime(-2, 1));
                        movers.Add(Mover.OneTime(-2, -1));
                        movers.Add(Mover.OneTime(1, 2));
                        movers.Add(Mover.OneTime(1, -2));
                        movers.Add(Mover.OneTime(-1, 2));
                        movers.Add(Mover.OneTime(-1, -2));
                        break;
                    case Piece.WhiteBishop:
                    case Piece.BlackBishop:
                        movers.Add(Mover.Continuous(1, 1));
                        movers.Add(Mover.Continuous(1, -1));
                        movers.Add(Mover.Continuous(-1, 1));
                        movers.Add(Mover.Continuous(-1, -1));
                        break;
                    case Piece.WhiteRook:
                    case Piece.BlackRook:
                        movers.Add(Mover.Continuous(1, 0));
                        movers.Add(Mover.Continuous(0, 1));
                        movers.Add(Mover.Continuous(-1, 0));
                        movers.Add(Mover.Continuous(0, -1));
                        break;
                    case Piece.WhiteKing:
                    case Piece.BlackKing:
                        movers.Add(Mover.OneTime(1, 1));
                        movers.Add(Mover.OneTime(1, -1));
                        movers.Add(Mover.OneTime(-1, 1));
                        movers.Add(Mover.OneTime(-1, -1));
                        movers.Add(Mover.OneTime(1, 0));
                        movers.Add(Mover.OneTime(0, 1));
                        movers.Add(Mover.OneTime(-1, 0));
                        movers.Add(Mover.OneTime(0, -1));
                        break;
                    case Piece.WhiteQueen:
                    case Piece.BlackQueen:
                        movers.Add(Mover.Continuous(1, 1));
                        movers.Add(Mover.Continuous(1, -1));
                        movers.Add(Mover.Continuous(-1, 1));
                        movers.Add(Mover.Continuous(-1, -1));
                        movers.Add(Mover.Continuous(1, 0));
                        movers.Add(Mover.Continuous(0, 1));
                        movers.Add(Mover.Continuous(-1, 0));
                        movers.Add(Mover.Continuous(0, -1));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                foreach (var mover in movers)
                {
                    var moves = LegalMoves(position, mover, cell, canCature);
                    allMoves.AddRange(moves);
                }
            }

            return allMoves;
        }

        private static List<MoveModel> LegalMoves(PositionModel position, Mover mover, CellPieceModel initial, bool canCapture = true)
        {
            var legalMoves = new List<MoveModel>();
            var piece = initial.Piece;

            var initialCell = initial.Cell.Clone();
            while (true)
            {
                var targetCell = mover.Move(initialCell);
                ///Check that we're still on the board
                ///
                if (targetCell == null)
                {
                    break;
                }

                var targetPiece = position.Get(targetCell);

                ///Destination cell is empty
                if (targetPiece == null)
                {
                    legalMoves.Add(new MoveModel()
                    {
                        Added = new List<CellPieceModel>() {new CellPieceModel(targetCell, piece)}, Removed = new List<CellPieceModel> {new CellPieceModel(initial.Cell, piece)}
                    });
                }
                else if (!piece.Value.IsSameColor(targetPiece.Value) && canCapture)
                {
                    legalMoves.Add(new MoveModel()
                    {
                        Added = new List<CellPieceModel>() {new CellPieceModel(targetCell, piece)}, Removed = new List<CellPieceModel>
                        {
                            new CellPieceModel(initial.Cell, piece), new CellPieceModel(targetCell, targetPiece)
                        }
                    });

                    break;
                }
                else
                {
                    break;
                }

                initialCell = targetCell;
            }

            return legalMoves;
        }
    }

    class Mover
    {
        private readonly int _dx;
        private readonly int _dy;
        private readonly bool isContinuous;
        private bool isActive = true;

        public static Mover Continuous(int dx, int dy)
        {
            return new Mover(dx, dy, true);
        }

        public static Mover OneTime(int dx, int dy)
        {
            return new Mover(dx, dy, false);
        }

        private Mover(int dx, int dy, bool isContinuous)
        {
            _dx = dx;
            _dy = dy;
            this.isContinuous = isContinuous;
            func = model => model.Move(dx, dy);
        }

        public CellModel Move(CellModel cell)
        {
            if (!isActive)
            {
                return null;
            }

            if (!isContinuous)
            {
                isActive = false;
            }

            return func(cell);
        }

        private Func<CellModel, CellModel> func;
    }
}