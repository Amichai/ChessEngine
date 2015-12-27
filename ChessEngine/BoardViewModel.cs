using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ChessKit.ChessLogic;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

namespace ChessEngine
{
    public class BoardViewModel : INotifyPropertyChanged
    {
        private Column[] _Columns;

        private MoveList _MoveList;

        public Subject<Position> NewPosition;
        private readonly PromotionDialog promotionDialog;

        private Cell SelectedCell;
        private PieceType selectedPiece;
        private readonly ManualResetEvent waitingForUserSelection;

        public void Reset()
        {
            this.Position = Fen.StartingPosition;
            NewPosition = new Subject<Position>();
            Columns = new Column[8];
            for (var i = 0; i < 8; i++)
            {
                Columns[i] = new Column(this.promotionDialog);
                Columns[i].CellSelected.Subscribe(cell =>
                {
                    if (this.promotionDialog.DialogVisible)
                    {
                        return;
                    }
                    deselectAllBut(cell.ID);
                    removeAllHighlights();
                    SelectedCell = cell;
                    if (cell.Piece.Color != this.MoveList.NextTurn)
                    {
                        return;
                    }
                    highlightAvailableCells(cell);
                });
                Columns[i].CellDeselected.Subscribe(cell =>
                {
                    if (this.promotionDialog.DialogVisible)
                    {
                        return;
                    }
                    if (cell.ID == SelectedCell.ID)
                    {
                        SelectedCell = null;
                    }
                    removeAllHighlights();
                });
                Columns[i].MoveTo.Subscribe(cell =>
                {
                    if (this.promotionDialog.DialogVisible)
                    {
                        return;
                    }
                    Task.Run(() => { pieceMoved(cell); });
                });
            }
            MoveList = new MoveList();
        }

        public BoardViewModel(PromotionDialog promotionDialog)
        {
            this.promotionDialog = promotionDialog;
            waitingForUserSelection = new ManualResetEvent(false);
            this.promotionDialog.PieceSelected.Subscribe(i =>
            {
                selectedPiece = i;
                waitingForUserSelection.Set();
            });
            this.Reset();
        }

        public Position Position
        {
            get; private set;
        }

        public FSharpList<LegalMove> LegalMoves()
        {
            return GetLegalMoves.All(this.Position);
        }

        public BoardState BoardState
        {
            get
            {
                Piece[][] pieces = new Piece[8][];
                for (var i = 0; i < 8; i++)
                {
                    pieces[i] = Columns[i].GetPieces();
                }
                return new BoardState(pieces, this.MoveList);
            }
        }

        public MoveList MoveList
        {
            get { return _MoveList; }
            set
            {
                if (_MoveList != value)
                {
                    _MoveList = value;
                    OnPropertyChanged("MoveList");
                }
            }
        }

        public Column[] Columns
        {
            get { return _Columns; }
            set
            {
                if (_Columns != value)
                {
                    _Columns = value;
                    OnPropertyChanged("Columns");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void removePiece(int col, int row)
        {
            Columns[col - 1].Cells[8 - row - 1].Piece = null;
        }

        private void placePiece(int col, int row, Piece piece)
        {
            Columns[col - 1].Cells[8 - row - 1].Piece = piece;
        }

        private void pieceMoved(Cell cell)
        {
            var start = SelectedCell.Coordinate;
            var end = cell.Coordinate;
            var m = new Move(new Tuple<int, int>(start.col, start.row),
                new Tuple<int, int>(end.col, end.row),
                new FSharpOption<ChessKit.ChessLogic.PieceType>(null) { });

            var selectedPiece = SelectedCell.Piece;
            this.ExecuteMove(m, selectedPiece.Color);
            NewPosition.OnNext(this.Position);
        }

        private void removeAllHighlights()
        {
            foreach (var c in Columns)
            {
                c.Cells.ToList().ForEach(i => i.Highlighted = false);
            }
        }

        private void highlightAvailableCells(Cell cell)
        {
            var cells = GetLegalMoves.FromSquare(cell.col, cell.row, this.Position);

            foreach (var c in cells)
            {
                Columns[c.Move.End.Item1].Highlight(c.Move.End.Item2);
            }
        }

        private void deselectAllBut(int id)
        {
            foreach (var c in Columns)
            {
                c.DeselectAllBut(id);
            }
        }

        private void OnPropertyChanged(string name)
        {
            var eh = PropertyChanged;
            if (eh != null)
            {
                eh(this, new PropertyChangedEventArgs(name));
            }
        }

        private static Piece TranslatePieceType(SideColor color, ChessKit.ChessLogic.PieceType p)
        {
            if (p.IsRook)
            {
                return new Piece(color, PieceType.Rook);
            }
            if(p.IsKnight)
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
            if(p.IsPawn)
            {
                return new Piece(color, PieceType.Pawn);
            }
            throw new Exception();
        }

        public void ExecuteMove(Move m, Color color)
        {
            this.ExecuteMove(m, color == Color.Black ? SideColor.Black : SideColor.White);
        }

        public void ExecuteMove(Move m, SideColor color)
        {
            var move = this.Position.ValidateLegalMove(m);
            var start = move.Move.Start;
            var end = move.Move.End;
            Columns[start.Item1].Cells[start.Item2].Piece = null;
            Columns[end.Item1].Cells[end.Item2].Piece = TranslatePieceType(color, move.Piece);
            var a = move.ToPosition();
            this.Position = a;
        }

        internal void ExecuteMove(SingleMove i)
        {
            var start = i.Start;
            var end = i.End;

            var colDiff = start.col - end.col;
            if (i.Piece == PieceType.King && Math.Abs(colDiff) == 2)
            {
                if (colDiff == -2)
                {
                    var rook = Columns[7].Cells[start.row];
                    Columns[start.col + 1].Cells[start.row].Piece = rook.Piece;
                    rook.Piece = null;
                }
                else
                {
                    var rook = Columns[0].Cells[start.row];
                    Columns[start.col - 1].Cells[start.row].Piece = rook.Piece;
                    rook.Piece = null;
                }
            }

            Columns[start.col].Cells[start.row].Piece = null;
            Columns[end.col].Cells[end.row].Piece = new Piece(i.SideColor, i.Piece);
            Application.Current.Dispatcher.Invoke(() => { MoveList.Add(i); });
        }

        internal string ToFEN()
        {
            return this.BoardState.ToFEN();
        }
    }
}