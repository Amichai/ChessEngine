using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using ChessKit.ChessLogic;
using Microsoft.FSharp.Core;


namespace ChessEngine
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private BoardViewModel _BoardViewModel;

        public MainWindow()
        {
            InitializeComponent();
            var stockFish = new Stockfish();
            MoveEval = new List<Tuple<string, double>>();
            BoardViewModel = new BoardViewModel(promotionDialog);
            BoardViewModel.AnalysisProgress.Subscribe(i => this.AnalysisProgressVal = i);
            BoardViewModel.NewPosition.Subscribe(i =>
            {
                var result = stockFish.AnalyzePosition(i);
                this.Eval = result.Eval;
                if (this.BoardViewModel.MoveList.NextTurn == SideColor.Black)
                {
                    this.Eval *= -1;
                }
                if (!this.IsModeStockfish)
                {
                    return;
                }
                var start = result.Start;
                var end = result.End;
                var promoteTo = i.Move.Value.Move.PromoteTo;
                var m = new Move(new Tuple<int, int>(start.col, start.row),
                    new Tuple<int, int>(end.col, end.row),
                    promoteTo);

                this.BoardViewModel.ExecuteMove(m, i.Core.ActiveColor);
                AnalyzePosition();
            });
        }

        private double _Eval;
        public double Eval
        {
            get { return _Eval; }
            set
            {
                _Eval = value;
                OnPropertyChanged("Eval");
            }
        }

        private bool _IsModeStockfish = true;
        public bool IsModeStockfish
        {
            get { return this._IsModeStockfish; }
            set { this._IsModeStockfish = value; }
        }

        public BoardViewModel BoardViewModel
        {
            get { return _BoardViewModel; }
            set
            {
                if (_BoardViewModel != value)
                {
                    _BoardViewModel = value;
                    OnPropertyChanged("BoardViewModel");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            var eh = PropertyChanged;
            if (eh != null)
            {
                eh(this, new PropertyChangedEventArgs(name));
            }
        }

        private void Back_OnClick(object sender, RoutedEventArgs e)
        {
            var fen = this.BoardViewModel.ToFEN();
        }

        private void Restart_OnClick(object sender, RoutedEventArgs e)
        {
            this.BoardViewModel.Reset();
        }

        private List<Tuple<string, double>> _MoveEval;
        public List<Tuple<string, double>> MoveEval
        {
            get { return _MoveEval; }
            set
            {
                if (_MoveEval != value)
                {
                    _MoveEval = value;
                    OnPropertyChanged("MoveEval");
                }
            }
        }

        private double _AnalysisProgressVal;

        public double AnalysisProgressVal
        {
            get { return _AnalysisProgressVal; }
            set
            {
                _AnalysisProgressVal = value;
                OnPropertyChanged("AnalysisProgressVal");
            }
        }

        private async void AnalyzePosition_OnClick(object sender, RoutedEventArgs e)
        {
            await AnalyzePosition();
        }

        private async Task AnalyzePosition()
        {
            var moveEval = await BoardViewModel.AnalyzePosition();
            this.MoveEval = null;

            this.MoveEval =
                new List<Tuple<string, double>>(
                    moveEval.Keys.Select(i => new Tuple<string, double>(MovePrinter.Print(i), moveEval[i])));
            foreach (var move in moveEval.Keys)
            {
                var text = MovePrinter.Print(move);
                Debug.Print(text + " - " + moveEval[move]);
            }
        }
    }
}