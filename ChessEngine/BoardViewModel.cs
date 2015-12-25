using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ChessEngine
{
    public class BoardViewModel : INotifyPropertyChanged
    {
        private Column[] _Columns;

        private MoveList _MoveList;

        public Subject<BoardState> NewPosition;
        private readonly PromotionDialog promotionDialog;

        private Cell SelectedCell;
        private PieceType selectedPiece;
        private readonly ManualResetEvent waitingForUserSelection;

        public void Reset()
        {
            NewPosition = new Subject<BoardState>();
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

        private BoardState boardState
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

        private void pieceMoved(Cell cell)
        {
            var start = SelectedCell.Coordinate;
            var end = cell.Coordinate;
            var selectedPiece = SelectedCell.Piece;
            var move = new SingleMove(selectedPiece.PieceType, start, end, selectedPiece.Color, cell.Piece);

            if (MoveList.Last != null && move.End == this.MoveList.Last.EnPassantTarget)
            {
                var lastEnd = move.End;
                if (move.SideColor == SideColor.Black)
                {
                    Columns[lastEnd.Col - 1].Cells[8 - lastEnd.Row - 1].Piece = null;
                }
                else
                {
                    Columns[lastEnd.Col - 1].Cells[8 - lastEnd.Row + 1].Piece = null;
                }
            }

            cell.Piece = SelectedCell.Piece;
            SelectedCell.Piece = null;
            removeAllHighlights();
            deselectAllBut(cell.ID);

            ///Check if we are promoting a pawn
            if (selectedPiece.PieceType == PieceType.Pawn &&
                ((selectedPiece.Color == SideColor.Black && cell.Row == 7) ||
                 (selectedPiece.Color == SideColor.White && cell.Row == 0)))
            {
                promotionDialog.White = selectedPiece.Color == SideColor.White;
                promotionDialog.DialogVisible = true;

                waitingForUserSelection.WaitOne();
                waitingForUserSelection.Reset();

                cell.Piece.PieceType = this.selectedPiece;
                promotionDialog.DialogVisible = false;
                move.Promotion = this.selectedPiece;
            }

            var colDiff = move.End.col - move.Start.col;
            if (move.Piece == PieceType.King && Math.Abs(colDiff) == 2)
            {
                if (colDiff == 2)
                {
                    var rook = Columns[7].Cells[move.Start.row];
                    Columns[move.Start.col + 1].Cells[move.Start.row].Piece = rook.Piece;
                    rook.Piece = null;
                }
                else
                {
                    var rook = Columns[0].Cells[move.Start.row];
                    Columns[move.Start.col - 1].Cells[move.Start.row].Piece = rook.Piece;
                    rook.Piece = null;
                }
            }

            Application.Current.Dispatcher.Invoke(() => { MoveList.Add(move); });
            NewPosition.OnNext(boardState);
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
            var cellsToHighlight = cell.GetAvailableCells(boardState, MoveList);
            foreach (var c in cellsToHighlight)
            {
                Columns[c.col - 1].Highlight(8 - c.row);
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

        internal void ExecuteMove(SingleMove i)
        {
            var start = i.Start;
            var end = i.End;
            Columns[start.col].Cells[start.row].Piece = null;
            Columns[end.col].Cells[end.row].Piece = new Piece(i.SideColor, i.Piece);
            Application.Current.Dispatcher.Invoke(() => { MoveList.Add(i); });
        }

        internal string ToFEN()
        {
            return this.boardState.ToFEN();
        }
    }
}