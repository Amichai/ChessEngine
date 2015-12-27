using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System;


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
                var result = stockFish.AnalyzePosition(i.ToFEN());
                this.Eval = result.Eval;
                if (this.BoardViewModel.MoveList.NextTurn == SideColor.Black)
                {
                    this.Eval *= -1;
                }
                this.BoardViewModel.ExecuteMove(new SingleMove(result.Start, result.End, BoardViewModel.BoardState));
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