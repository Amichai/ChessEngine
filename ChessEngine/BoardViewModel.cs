using System;
using System.Collections.Generic;
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

        private List<IDisposable> subscriptions = new List<IDisposable>();
        public void Reset()
        {
            this.subscriptions.ForEach(s => s.Dispose());
            this.subscriptions.Clear();

            this.Position = Fen.StartingPosition;
            NewPosition = new Subject<Position>();
            Columns = new Column[8];
            for (var i = 0; i < 8; i++)
            {
                Columns[i] = new Column(this.promotionDialog);
                var s1 = Columns[i].CellSelected.Subscribe(cell =>
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
                var s2 = Columns[i].CellDeselected.Subscribe(cell =>
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
                var s3 = Columns[i].MoveTo.Subscribe(cell =>
                {
                    if (this.promotionDialog.DialogVisible)
                    {
                        return;
                    }
                    removeAllHighlights();
                    Task.Run(() => { pieceMoved(cell); });
                });

                subscriptions.Add(s1);
                subscriptions.Add(s2);
                subscriptions.Add(s3);
            }
            MoveList = new MoveList();
        }

        public BoardViewModel(PromotionDialog promotionDialog)
        {
            this.promotionDialog = promotionDialog;

            if (promotionDialog != null)
            {
                waitingForUserSelection = new ManualResetEvent(false);
                this.promotionDialog.PieceSelected.Subscribe(i =>
                {
                    selectedPiece = i;
                    waitingForUserSelection.Set();
                });
            }

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

        private void RemovePiece(int col, int row)
        {
            Columns[col - 1].Cells[8 - row - 1].Piece = null;
        }

        private void PlacePiece(int col, int row, Piece piece)
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
            this.ExecuteMove(m);
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

        public void ExecuteMove(Move m, Color color)
        {
            this.ExecuteMove(m);
        }

        private void addToMoveList(Move m)
        {
            var s = m.Start;
            var e = m.End;
            var start = new CellCoordinate(s.Item1, s.Item2);
            var end = new CellCoordinate(e.Item1, e.Item2);
            var singleMove = new SingleMove(start, end, BoardState);
            this.MoveList.Add(singleMove);
        }

        private Cell Get(int c, int r)
        {
            return Columns[c].Cells[r];
        }

        private Cell Get(Tuple<int, int> p)
        {
            return Get(p.Item1, p.Item2);
        }

        private void MovePiece(Cell from, Cell to)
        {
            var piece = from.Piece;
            from.Piece = null;
            to.Piece = piece;
        }

        public void ExecuteMove(Move m)
        {
            this.addToMoveList(m);
            var legal = this.Position.ValidateLegalMove(m);
            var start = Get(legal.Move.Start);
            var end = Get(legal.Move.End);
            if (start.Piece.PieceType == PieceType.King && Math.Abs(start.col - end.col) == 2)
            {
                var row = legal.Move.Start.Item2;
                if (end.col == 6)
                {
                    var r1 = Get(7, row);
                    var r2 = Get(end.col - 1, row);
                    MovePiece(r1, r2);
                }
                else
                {
                    var r1 = Get(0, row);
                    var r2 = Get(end.col + 1, row);
                    MovePiece(r1, r2);
                }
            }
            MovePiece(start, end);
            var a = legal.ToPosition();
            this.Position = a;
        }

        internal string ToFEN()
        {
            return this.BoardState.ToFEN();
        }

        private readonly PositionAnalyzer positionAnalyzer = new PositionAnalyzer();

        public Task<PositionAnalysis> AnalyzePosition()
        {
            return positionAnalyzer.Analyze(this.Position);
        }

        public Subject<double> AnalysisProgress
        {
            get { return positionAnalyzer.Progress; }
        }
    }
}