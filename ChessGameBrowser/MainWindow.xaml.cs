using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChessEngine;
using ChessKit.ChessLogic;

namespace ChessGameBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            BoardViewModel = new BoardViewModel(null);
            InitializeComponent();

            moves = GameString.Split(' ').Select(i => i.Trim()).Where(i => !char.IsDigit(i.First())).ToList();

        }


        private int moveCounter = 0;
        private List<string> moves;

        private BoardViewModel _BoardViewModel;
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

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (moveCounter == 0)
            {
                return;
            }

            moveCounter--;
            BoardViewModel = new BoardViewModel(null);
            for (int i = 0; i < moveCounter; i++)
            {
                var s1 = moves[i];
                var m = San.Parse(s1, BoardViewModel.Position);
                BoardViewModel.ExecuteMove(m.Move);
            }
        }

        private void Forward_click(object sender, RoutedEventArgs e)
        {
            if (moveCounter >= moves.Count)
            {
                return;
            }

            var s1 = moves[moveCounter++];

            var m = San.Parse(s1, BoardViewModel.Position);
            BoardViewModel.ExecuteMove(m.Move);
        }
    }
}
