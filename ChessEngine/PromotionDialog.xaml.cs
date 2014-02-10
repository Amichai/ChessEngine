using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
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

namespace ChessEngine {
    /// <summary>
    /// Interaction logic for PromotionDialog.xaml
    /// </summary>
    public partial class PromotionDialog : UserControl, INotifyPropertyChanged {
        public PromotionDialog() {
            InitializeComponent();
            this.DialogVisible = false;
            this.PieceSelected = new Subject<PieceType>();
        }

        public Subject<PieceType> PieceSelected;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) {
            var eh = PropertyChanged;
            if (eh != null) {
                eh(this, new PropertyChangedEventArgs(name));
            }
        }

        private bool _White;
        public bool White {
            get { return _White; }
            set {
                if (_White != value) {
                    _White = value;
                    OnPropertyChanged("White");
                    OnPropertyChanged("Black");
                }
            }
        }

        private bool _DialogVisible;
        public bool DialogVisible {
            get { return _DialogVisible; }
            set {
                if (_DialogVisible != value) {
                    _DialogVisible = value;
                    OnPropertyChanged("DialogVisible");
                }
            }
        }

        public bool Black {
            get {
                return !this.White;
            }
        }

        private void Image_PreviewMouseDown_1(object sender, MouseButtonEventArgs e) {
            var selected = (sender as Image).Tag as string;
            switch (selected) {
                case "Knight":
                    this.PieceSelected.OnNext(PieceType.Knight);
                    break;
                case "Bishop":
                    this.PieceSelected.OnNext(PieceType.Bishop);
                    break;
                case "Rook":
                    this.PieceSelected.OnNext(PieceType.Rook);
                    break;
                case "Queen":
                    this.PieceSelected.OnNext(PieceType.Queen);
                    break;
            }
        }
    }
}
