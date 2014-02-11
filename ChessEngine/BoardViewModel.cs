using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ChessEngine {
    public class BoardViewModel : INotifyPropertyChanged {
        PromotionDialog promotionDialog;
        public BoardViewModel(PromotionDialog promotionDialog) {
            this.promotionDialog = promotionDialog;
            this.waitingForUserSelection = new ManualResetEvent(false);
            this.NewPosition = new Subject<BoardState>();
            this.promotionDialog.PieceSelected.Subscribe(i => {
                this.selectedPiece = i;
                this.waitingForUserSelection.Set();
            });
            this.Columns = new Column[8];
            for (int i = 0; i < 8; i++) {
                this.Columns[i] = new Column(this.promotionDialog);
                this.Columns[i].CellSelected.Subscribe(cell => {
                    if (this.promotionDialog.DialogVisible) {
                        return;
                    }
                    deselectAllBut(cell.ID);
                    removeAllHighlights();
                    highlightAvailableCells(cell);
                    this.SelectedCell = cell;
                });
                this.Columns[i].CellDeselected.Subscribe(cell => {
                    if (this.promotionDialog.DialogVisible) {
                        return;
                    }
                    if (cell.ID == SelectedCell.ID) {
                        this.SelectedCell = null;
                    }
                    removeAllHighlights();
                });
                this.Columns[i].MoveTo.Subscribe(cell => {
                    if (this.promotionDialog.DialogVisible) {
                        return;
                    }
                    Task.Run(() => { pieceMoved(cell); });
                });
            }
            this.MoveList = new MoveList();
        }

        private void pieceMoved(Cell cell) {
            var start = SelectedCell.Coordinate;
            var end = cell.Coordinate;
            var selectedPiece = SelectedCell.Piece;
            ///Code duplication between this method and PieceExtensions.ApplyMove()
            var move = new SingleMove() {
                Piece = selectedPiece.PieceType,
                End = end,
                Start = start,
                SideColor = selectedPiece.Color,
                Taken = cell.Piece,
            };

            ///Check if we took en passant:
            if (selectedPiece.PieceType == PieceType.Pawn && start.Col != end.Col && cell.Piece == null) {
                if (selectedPiece.Color == SideColor.Black) {
                    this.Columns[cell.col].Cells[cell.row - 1].Piece = null;
                } else {
                    this.Columns[cell.col].Cells[cell.row + 1].Piece = null;
                }
            }

            cell.Piece = SelectedCell.Piece;
            this.SelectedCell.Piece = null;
            removeAllHighlights();
            deselectAllBut(cell.ID);

            ///Check if we are promoting a pawn
            if (selectedPiece.PieceType == PieceType.Pawn &&
                ((selectedPiece.Color == SideColor.Black && cell.row == 7) ||
                (selectedPiece.Color == SideColor.White && cell.row == 0))) {
                this.promotionDialog.White = selectedPiece.Color == SideColor.White;
                this.promotionDialog.DialogVisible = true;

                this.waitingForUserSelection.WaitOne();
                this.waitingForUserSelection.Reset();

                cell.Piece.PieceType = this.selectedPiece;
                this.promotionDialog.DialogVisible = false;
                move.Promotion = this.selectedPiece;
            }

            var colDiff = move.End.Col - move.Start.Col;
            if (move.Piece == PieceType.King && Math.Abs(colDiff) == 2) {
                if (colDiff == 2) {
                    var rook = this.Columns[7].Cells[move.Start.Row];
                    this.Columns[move.Start.Col + 1].Cells[move.Start.Row].Piece = rook.Piece;
                    rook.Piece = null;
                } else {
                    var rook = this.Columns[0].Cells[move.Start.Row];
                    this.Columns[move.Start.Col - 1].Cells[move.Start.Row].Piece = rook.Piece;
                    rook.Piece = null;
                }
            }

            App.Current.Dispatcher.Invoke(((Action)(() => {
                this.MoveList.Add(move);
            })));
            this.NewPosition.OnNext(new BoardState(this.boardState));
        }

        private Cell SelectedCell;
        private ManualResetEvent waitingForUserSelection;
        private PieceType selectedPiece;

        private void removeAllHighlights() {
            foreach (var c in Columns) {
                c.Cells.ToList().ForEach(i => i.Highlighted = false);
            }
        }

        public Subject<BoardState> NewPosition;

        private Piece[][] boardState {
            get {
                Piece[][] toReturn = new Piece[8][];
                for (int i = 0; i < 8; i++) {
                    toReturn[i] = this.Columns[i].GetPieces();
                }
                return toReturn;
            }
        }

        private MoveList _MoveList;
        public MoveList MoveList {
            get { return _MoveList; }
            set {
                if (_MoveList != value) {
                    _MoveList = value;
                    OnPropertyChanged("MoveList");
                }
            }
        }

        private void highlightAvailableCells(Cell cell) {
            List<CellCoordinate> cellsToHighlight = cell.GetAvailableCells(this.boardState, this.MoveList);
            foreach (var c in cellsToHighlight) {
                this.Columns[c.Col].Highlight(c.Row);
            }
        }

        private void deselectAllBut(int id) {
            foreach (var c in Columns) {
                c.DeselectAllBut(id);
            }
        }

        private Column[] _Columns;
        public Column[] Columns {
            get { return _Columns; }
            set {
                if (_Columns != value) {
                    _Columns = value;
                    OnPropertyChanged("Columns");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) {
            var eh = PropertyChanged;
            if (eh != null) {
                eh(this, new PropertyChangedEventArgs(name));
            }
        }

        internal void ExecuteMove(SingleMove i) {
            var start = i.Start;
            var end = i.End;
            this.Columns[start.Col].Cells[start.Row].Piece = null;
            this.Columns[end.Col].Cells[end.Row].Piece = new Piece(i.SideColor, i.Piece);
            App.Current.Dispatcher.Invoke(((Action)(() => {
                this.MoveList.Add(i);
            })));
        }
    }
}
