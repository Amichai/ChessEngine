using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine {
    public class SingleMove {
        private static int moveCounter = 0;
        public SingleMove() {
            this.MoveNumber = ++moveCounter;
            this.Promotion = null;
        }
        public PieceType Piece { get; set; }
        public CellCoordinate Start { get; set; }
        public CellCoordinate End { get; set; }
        public SideColor SideColor { get; set; }
        public Piece Taken { get; set; }
        public int MoveNumber { get; private set; }

        public new string ToString {
            get {
                string toReturn = string.Format("{0}: {1}, {2}-{3}",
                        MoveNumber, Piece.ToShortString(), Start.Notation, End.Notation);
                if (Promotion != null) {
                    toReturn += string.Format("=({0})", Promotion.Value.ToShortString());
                }
                return toReturn;
            }
        }

        public PieceType? Promotion { get; set; }
    }
}
