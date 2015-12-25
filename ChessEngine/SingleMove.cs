using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine {
    public class SingleMove {
        private static int moveCounter = 0;
        public SingleMove(PieceType piece, CellCoordinate start, CellCoordinate end, SideColor color, Piece taken)
        {
            this.Piece = piece;
            this.Start = start;
            this.End = end;
            this.SideColor = color;
            this.Taken = taken;
            this.MoveNumber = ++moveCounter;
            this.Promotion = null;
            this.setEnPassantTarget();

        }
        public PieceType Piece { get; private set; }
        public CellCoordinate Start { get; private set; }
        public CellCoordinate End { get; private set; }
        public CellCoordinate EnPassantTarget { get; set; }
        public SideColor SideColor { get; private set; }
        public Piece Taken { get; private set; }
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

        private void setEnPassantTarget()
        {
            if (this.Piece == PieceType.Pawn &&
                (this.Start.row == 1 || Start.row == 6)
                && (End.row == 3 || End.row == 4))
            {
                int c = End.col;
                int r;
                switch (SideColor)
                {
                    case SideColor.Black:
                        r = End.row - 1;
                        break;
                    case SideColor.White:
                        r = End.row + 1;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                this.EnPassantTarget = new CellCoordinate(c, r);
            }
        }
    }
}
