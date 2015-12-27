using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine {
    [DebuggerDisplay("{col}, {row}")]
    public class CellCoordinate {
        public CellCoordinate(int col, int row) {
            this.col = col;
            this.row = row;
        }
        public int col { get; set; }
        public int row { get; set; }

        public string Notation {
            get {
                return (char)(97 + col) + (8 - row).ToString();
            }
        }


        public int Col
        {
            get { return this.col + 1; }
        }

        public int Row
        {
            get { return 8 - this.row; }
        }


        public override string ToString() {
            return string.Format("{0}, {1}", col, row);
        }

        public override bool Equals(System.Object obj) {
            // If parameter is null return false.
            if (obj == null) {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            CellCoordinate p = obj as CellCoordinate;
            if ((System.Object)p == null) {
                return false;
            }

            // Return true if the fields match:
            return (this.col == p.col) && (this.row == p.row);
        }

        public bool Equals(CellCoordinate p) {
            // If parameter is null return false:
            if ((object)p == null) {
                return false;
            }

            // Return true if the fields match:
            return (this.col == p.col) && (this.row == p.row);
        }

        public override int GetHashCode() {
            return this.col ^ this.row;
        }

        public static bool operator ==(CellCoordinate a, CellCoordinate b) {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b)) {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null)) {
                return false;
            }

            // Return true if the fields match:
            return a.col == b.col && a.row == b.row;
        }

        public static bool operator !=(CellCoordinate a, CellCoordinate b) {
            return !(a == b);
        }

        internal static CellCoordinate FromString(string p)
        {
            var c = ((int) p[0]) - 97;
            var r = 8 - int.Parse(p[1].ToString());
            return new CellCoordinate(c, r);
        }
    }
}
