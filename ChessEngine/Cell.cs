using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine {
    public class Cell : INotifyPropertyChanged {
        private Piece _Piece;
        public Piece Piece {
            get { return _Piece; }
            set {
                if (_Piece != value) {
                    _Piece = value;
                    OnPropertyChanged("Piece");
                }
            }
        }

        public bool Background { get; set; }
        private static int counter = 0;
        public int ID { get; set; }

        public PieceType PieceType {
            get {
                return Piece.PieceType;
            }
        }

        public int col {
            get {
                return ID / 8;
            }
        }

        public int row {
            get {
                return ID % 8;
            }
        }

        public Cell() {
            this.ID = counter++;
            this.Background = (col + row) % 2 != 0;
            this.Selected = false;
            this.Highlighted = false;
            this.CellSelected = new Subject<Cell>();
            this.CellDeselected = new Subject<Cell>();
            this.Piece = InitialBoardConfigurationFactory.Get(this.ID);
        }


        private bool _Selected;
        public bool Selected {
            get { return _Selected; }
            set {
                if (_Selected != value) {
                    _Selected = value;
                    OnPropertyChanged("Selected");
                }
            }
        }

        public Subject<Cell> CellSelected;
        public Subject<Cell> CellDeselected;

        internal void Select() {
            if (this.Piece == null) {
                return;
            }
            if (!this.Selected) {
                this.Selected = true;
                this.CellSelected.OnNext(this);
            } else {
                this.Selected = false;
                this.CellDeselected.OnNext(this);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) {
            var eh = PropertyChanged;
            if (eh != null) {
                eh(this, new PropertyChangedEventArgs(name));
            }
        }

        private bool _Highlighted;
        public bool Highlighted {
            get { return _Highlighted; }
            set {
                if (_Highlighted != value) {
                    _Highlighted = value;
                    OnPropertyChanged("Highlighted");
                }
            }
        }

        internal void Highlight() {
            this.Highlighted = true;
        }

        public CellCoordinate Coordinate {
            get {
                return new CellCoordinate(col, row);
            }
        }
    }
}
