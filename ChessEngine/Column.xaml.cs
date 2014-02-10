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
    /// Interaction logic for Row.xaml
    /// </summary>
    public partial class Column : UserControl, INotifyPropertyChanged {
        PromotionDialog promotionDialog;
        public Column(PromotionDialog promotionDialog) {
            this.promotionDialog = promotionDialog;
            InitializeComponent();
            this.CellSelected = new Subject<Cell>();
            this.CellDeselected = new Subject<Cell>();
            this.MoveTo = new Subject<Cell>();

            this.Cells = new Cell[8];
            for (int i = 0; i < 8; i++) {
                this.Cells[i] = new Cell();
                this.Cells[i].CellSelected.Subscribe(cell => {
                    this.CellSelected.OnNext(cell);
                });

                this.Cells[i].CellDeselected.Subscribe(cell => {
                    this.CellDeselected.OnNext(cell);
                });
            }
        }

        public Subject<Cell> CellSelected;
        public Subject<Cell> CellDeselected;
        public Subject<Cell> MoveTo;

        private Cell[] _Cells;
        public Cell[] Cells {
            get { return _Cells; }
            set {
                if (_Cells != value) {
                    _Cells = value;
                    OnPropertyChanged("Cells");
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

        internal void DeselectAllBut(int id) {
            foreach (var c in Cells) {
                if (c.ID == id) {
                    continue;
                } else {
                    c.Selected = false;
                }
            }
        }

        internal void Highlight(int rowIdx) {
            this.Cells[rowIdx].Highlight();
        }

        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            if (this.promotionDialog.DialogVisible) {
                return;
            }
            var cell = ((sender as Grid).Tag as Cell);
            if (cell.Highlighted) {
                this.MoveTo.OnNext(cell);
            } else {
                cell.Select();
            }
        }

        internal Piece[] GetPieces() {
            Piece[] toReturn = new Piece[8];
            for (int i = 0; i < 8; i++) {
                toReturn[i] = Cells[i].Piece;
			}
            return toReturn;
        }
    }
}
