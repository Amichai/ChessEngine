using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine {
    public class MoveList : ObservableCollection<SingleMove> {
        private bool whiteKingMoved = false;
        private bool blackKingMoved = false;
        private bool a1Moved = false;
        private bool h1Moved = false;
        private bool a8Moved = false;
        private bool h8Moved = false;

        public new void Add(SingleMove m) {
            if (m.Piece == PieceType.King) {
                if (m.SideColor == SideColor.Black) {
                    blackKingMoved = true;
                } else {
                    whiteKingMoved = true;
                }
            }
            var start = m.Start;
            if (start.Notation == "a1") {
                a1Moved = true;
            }
            if (start.Notation == "h1") {
                h1Moved = true;
            }
            if (start.Notation == "a8") {
                a8Moved = true;
            }
            if (start.Notation == "h8") {
                h8Moved = true;
            }
            base.Add(m);
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

        public SingleMove Last {
            get {
                if (this.Count() > 0) {
                    return this.Last();
                } else {
                    return null;
                }
            }
        }
    }
}
