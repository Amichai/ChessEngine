using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine {
    public enum PieceType { Bishop, Pawn, Knight, King, Queen, Rook };
    public enum SideColor { Black, White };
    public class Piece : INotifyPropertyChanged {
        private PieceType _PieceType;
        public PieceType PieceType {
            get { return _PieceType; }
            set {
                if (_PieceType != value) {
                    _PieceType = value;
                    OnPropertyChanged("PieceType");
                    OnPropertyChanged("AssetPath");
                }
            }
        }

        public SideColor Color { get; private set; }

        public Piece(SideColor color, PieceType type) {
            this.Color = color;
            this.PieceType = type;

            if (paths == null) {
                this.initializePathsDictionary();
            }
        }

        private void initializePathsDictionary() {
            paths = new Dictionary<SideColor, Dictionary<PieceType, string>>();
            paths[SideColor.Black] = new Dictionary<PieceType, string>() {
            {PieceType.Bishop, @"..\..\Assets\black_bishop.png"} ,
            {PieceType.King, @"..\..\Assets\black_king.png"} ,
            {PieceType.Knight, @"..\..\Assets\black_knight.png"} ,
            {PieceType.Rook, @"..\..\Assets\black_rook.png"} ,
            {PieceType.Pawn, @"..\..\Assets\black_pawn.png"} ,
            {PieceType.Queen, @"..\..\Assets\black_queen.png"} ,

            };
            paths[SideColor.White] = new Dictionary<PieceType, string>() {
            {PieceType.Bishop, @"..\..\Assets\white_bishop.png"} ,
            {PieceType.King, @"..\..\Assets\white_king.png"} ,
            {PieceType.Knight, @"..\..\Assets\white_knight.png"} ,
            {PieceType.Rook, @"..\..\Assets\white_rook.png"} ,
            {PieceType.Pawn, @"..\..\Assets\white_pawn.png"} ,
            {PieceType.Queen, @"..\..\Assets\white_queen.png"} ,

            };
        }

        public string ToFEN()
        {
            switch (Color)
            {
                case SideColor.Black:
                    switch (PieceType)
                    {
                        case PieceType.Bishop:
                            return "b";
                        case PieceType.Pawn:
                            return "p";
                        case PieceType.Knight:
                            return "n";
                        case PieceType.King:
                            return "k";
                        case PieceType.Queen:
                            return "q";
                        case PieceType.Rook:
                            return "r";
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case SideColor.White:
                    switch (PieceType)
                    {
                        case PieceType.Bishop:
                            return "B";
                        case PieceType.Pawn:
                            return "P";
                        case PieceType.Knight:
                            return "N";
                        case PieceType.King:
                            return "K";
                        case PieceType.Queen:
                            return "Q";
                        case PieceType.Rook:
                            return "R";
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new Exception();
            }
        }

        private static Dictionary<SideColor, Dictionary<PieceType, string>> paths = null;

        public string AssetPath
        {
            get { return paths[this.Color][this.PieceType]; }
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

        internal Piece Clone()
        {
            return new Piece(this.Color, this.PieceType);
        }
    }
}