using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine {
    public class MoveList {
        private bool whiteKingMoved = false;
        private bool blackKingMoved = false;
        private bool a1Moved = false;
        private bool h1Moved = false;
        private bool a8Moved = false;
        private bool h8Moved = false;

        private ObservableCollection<SingleMove> moves = new ObservableCollection<SingleMove>();

        public new void Add(SingleMove m)
        {
            if (m.Piece == PieceType.King)
            {
                if (m.SideColor == SideColor.Black)
                {
                    blackKingMoved = true;
                }
                else
                {
                    whiteKingMoved = true;
                }
            }
            var start = m.Start;
            if (start.Notation == "a1")
            {
                a1Moved = true;
            }
            if (start.Notation == "h1")
            {
                h1Moved = true;
            }
            if (start.Notation == "a8")
            {
                a8Moved = true;
            }
            if (start.Notation == "h8")
            {
                h8Moved = true;
            }

            if (m.Piece == PieceType.Pawn || m.Taken != null)
            {
                this.halfmoveClock++;
            }


            this.moves.Add(m);
        }

        public bool CanCastleLeft(SideColor color) {
            if (color == SideColor.White) {
                return !this.whiteKingMoved && !a1Moved;
            } else {
                return !this.blackKingMoved && !a8Moved;
            }
        }

        public bool CanCastleRight(SideColor color) {
            if (color == SideColor.White) {
                return !this.whiteKingMoved && !h1Moved;
            } else {
                return !this.blackKingMoved && !h8Moved;
            }
        }

        public string CastleStateFEN()
        {
            var toReturn = string.Empty;
            if (CanCastleLeft(SideColor.White))
            {
                toReturn += "Q";
            }
            if (CanCastleRight(SideColor.White))
            {
                toReturn += "K";
            }
            if (CanCastleLeft(SideColor.Black))
            {
                toReturn += "q";
            }
            if (CanCastleRight(SideColor.Black))
            {
                toReturn += "k";
            }
            return toReturn == string.Empty ? "-" : toReturn;
        }

        public int FullMoveNumber
        {
            get { return (int)Math.Ceiling(this.moves.Count / 2.0); }
        }

        public string EnPassantTargetFEN()
        {
            if (Last.Piece == PieceType.Pawn &&
                (Last.Start.Row == 1 || Last.Start.Row == 6)
                && (Last.End.Row == 3 || Last.End.Row == 4))
            {
                int c = Last.End.Col;
                int r;
                switch (Last.SideColor)
                {
                    case SideColor.Black:
                        r = Last.End.Row - 1;
                        break;
                    case SideColor.White:
                        r = Last.End.Row + 1;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return new CellCoordinate(c, r).Notation;
            }
            return "-";
        }

        private int halfmoveClock = 0;

        public int HalfMoveClock
        {
            get { return this.halfmoveClock; }
        }

        public SideColor NextTurn
        {
            get { return this.Last.SideColor == SideColor.White ? SideColor.Black : SideColor.White; }
        }

        public string NextTurnFEN
        {
            get
            {
                switch (NextTurn)
                {
                    case SideColor.Black:
                        return "b";
                    case SideColor.White:
                        return "w";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public SingleMove Last
        {
            get
            {
                if (this.moves.Count() > 0)
                {
                    return this.moves.Last();
                }
                else
                {
                    return null;
                }
            }
        }

        internal int Count()
        {
            return this.moves.Count;
        }
    }
}
