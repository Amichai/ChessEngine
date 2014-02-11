using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine {
    public class BoardState {
        public BoardState(Piece[][] state) {
            this.State = state;
        }
        public Piece[][] State { get; set; }

        public Dictionary<SideColor, Dictionary<PieceType, int>> OnTheBoard() {
            var pieces = new Dictionary<SideColor, Dictionary<PieceType, int>>();
            pieces[SideColor.White] = new Dictionary<PieceType,int>();
            pieces[SideColor.Black] = new Dictionary<PieceType,int>();

            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    var p = State[i][j];
                    if (p != null) {
                        int count;
                        if (pieces[p.Color].TryGetValue(p.PieceType, out count)) {
                            pieces[p.Color][p.PieceType] = count + 1;
                        } else {
                            pieces[p.Color][p.PieceType] = 1;
                        }
                    }
                }
            }
            return pieces;
        }

        public Piece[] this[int i] {
            get { return this.State[i]; }
            set { this.State[i] = value; }
        }

        public string ToString() {
            StringBuilder sb = new StringBuilder();
                for (int j = 0; j < 8; j++) {
            for (int i = 0; i < 8; i++) {
                    var p = this.State[i][j];
                    string toAppend = " ";
                    if (p != null) {
                        toAppend = p.PieceType.ToShortString();
                    }
                    sb.Append(toAppend);
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }

        internal BoardState Clone() {
            Piece[][] newPieces = new Piece[8][];
            for (int i = 0; i < 8; i++) {
                newPieces[i] = new Piece[8];
            }

            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    var p = this.State[i][j];
                    if (p != null) {
                        newPieces[i][j] = p.Clone();
                    }
                }
            }

            return new BoardState(newPieces);
        }
    }
}
