using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
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
        private Stockfish stockfish;

        public MainWindow()
        {
            InitializeComponent();
            stockfish = new Stockfish();
            MoveEval = new List<Tuple<string, double>>();
            BoardViewModel = new BoardViewModel(promotionDialog);
            BoardViewModel.AnalysisProgress.Subscribe(i => this.AnalysisProgressVal = i);
            BoardViewModel.NewPosition.Subscribe(i =>
            {
                var result = computePositionVal(i);
                
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

        private bool _IsModeTacticFinder;
        public bool IsModeTacticFinder
        {
            get { return _IsModeTacticFinder; }
            set
            {
                _IsModeTacticFinder = value;
                OnPropertyChanged("IsModeTacticFinder");
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

        private string _GameString =
            "1. e4 c5 2. Nf3 d6 3. d4 cxd4 4. Nxd4 Nf6 5. Nc3 a6 6. Be2 e5 7. Nb3 Be7 8. Be3 Be6 9. Nd5 Nbd7 10. Qd3 Bxd5 11. exd5 Nc5 12. Nxc5 dxc5 13. Bf3 Bd6 14. g4 O-O 15. g5 e4 16. Bxe4 Nxe4 17. Qxe4 Re8 18. Qg4 Qa5+ 19. c3 Re5 20. O-O Rxd5 21. c4 Re5 22. Qd7 Qb6 23. Rad1 Rd8 24. Qg4 Qc7 25. h4 Rde8 26. Qh3 Re4 27. b3 b5 28. cxb5 axb5 29. Rd5 c4 30. Rxb5 c3 31. Bc1 Qc6 32. Ra5 c2 33. Qf3 Bh2+ 34. Kh1 Bc7 35. Rf5 Rxh4+ 36. Kg1 Qd6 37. Qg3 Rg4 38. Qxg4 Qh2# 0-1";
        public string GameString
        {
            get { return _GameString; }
            set
            {
                _GameString = value;
                OnPropertyChanged("GameString");
            }
        }

        private AnalysisResult computePositionVal(Position p)
        {
            var result = stockfish.AnalyzePosition(p);
            this.Eval = result.Eval;
            if (this.BoardViewModel.MoveList.NextTurn == SideColor.Black)
            {
                this.Eval *= -1;
            }

            return result;
        }

        private async void Process_OnClick(object sender, RoutedEventArgs e)
        {
            var moves = GameString.Split(' ').Select(i => i.Trim());
            foreach (var s1 in moves)
            {
                if (char.IsDigit(s1.First()))
                {
                    continue;
                }
                var m = San.Parse(s1, BoardViewModel.Position);
                BoardViewModel.ExecuteMove(m.Move);
                computePositionVal(BoardViewModel.Position);
                await AnalyzePosition();
            }
        }
    }
}