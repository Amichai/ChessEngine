using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System;
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
            BoardViewModel = new BoardViewModel(promotionDialog);
            BoardViewModel.NewPosition.Subscribe(i =>
            {
                var result = stockFish.AnalyzePosition(i);
                this.Eval = result.Eval;
                if (this.BoardViewModel.MoveList.NextTurn == SideColor.Black)
                {
                    this.Eval *= -1;
                }
                var start = result.Start;
                var end = result.End;
                var m = new Move(new Tuple<int, int>(start.col, start.row),
                    new Tuple<int, int>(end.col, end.row),
                    new FSharpOption<ChessKit.ChessLogic.PieceType>(null) { });

                this.BoardViewModel.ExecuteMove(m, i.Core.ActiveColor);
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
    }
}