using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine {
    public class BoardState {
        private readonly MoveList _moveList;

        public BoardState(Piece[][] state, MoveList moveList)
        {
            if (moveList == null)
            {
                throw new ArgumentNullException("moveList");
            }
            _moveList = moveList;
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

        private Piece[] this[int i] {
            get { return this.State[i]; }
            set { this.State[i] = value; }
        }

        public Piece Remove(int col, int row)
        {
            var toReturn = this[col - 1][8 - row];
            this[col - 1][8 - row] = null;
            return toReturn;
        }

        public void Set(int col, int row, Piece piece)
        {
            this[col - 1][8 - row] = piece;
        }

        public Piece Get(int col, int row)
        {
            return this[col - 1][8 - row];
        }

        public void Move(int col1, int row1, int col2, int row2)
        {
            var p = this.Remove(col1, col2);
            this.Set(col2, row2, p);
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

            return new BoardState(newPieces, this._moveList);
        }



        internal string ToFEN()
        {
            List<string> rows = new List<string>();
            int moveCount = this._moveList.Count();
            var nextTurnColor = this._moveList.NextTurn;
            for (int i = 0; i < 8; i++)
            {
                string row = string.Empty;
                int emptyCount = 0;
                for (int j = 0; j < 8; j++)
                {
                    var p = this.State[j][i];
                    if (p == null)
                    {
                        emptyCount++;
                        continue;
                    }
                    if (emptyCount > 0)
                    {
                        row += emptyCount.ToString();
                        emptyCount = 0;
                    }
                    row += p.ToFEN();
                }
                if (emptyCount > 0)
                {
                    row += emptyCount.ToString();
                }
                rows.Add(row);
            }
            var layout = string.Join("/", rows);
            var toReturn = layout + " " + this._moveList.NextTurnFEN + " " + _moveList.CastleStateFEN() + " " +
                           _moveList.EnPassantTargetFEN() + " " + _moveList.HalfMoveClock + " " + _moveList.FullMoveNumber;
            Debug.Print(toReturn);
            return toReturn;
        }
    }
}
