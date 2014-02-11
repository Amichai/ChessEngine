using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine {
    [DebuggerDisplay("{Col}, {Row}")]
    public class CellCoordinate {
        public CellCoordinate(int col, int row) {
            this.Col = col;
            this.Row = row;
        }
        public int Col { get; set; }
        public int Row { get; set; }

        public string Notation {
            get {
                return (char)(97 + Col) + (8 - Row).ToString();
            }

        }

        public override string ToString() {
            return string.Format("{0}, {1}", Col, Row);
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
            return (this.Col == p.Col) && (this.Row == p.Row);
        }

        public bool Equals(CellCoordinate p) {
            // If parameter is null return false:
            if ((object)p == null) {
                return false;
            }

            // Return true if the fields match:
            return (this.Col == p.Col) && (this.Row == p.Row);
        }

        public override int GetHashCode() {
            return this.Col ^ this.Row;
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
            return a.Col == b.Col && a.Row == b.Row;
        }

        public static bool operator !=(CellCoordinate a, CellCoordinate b) {
            return !(a == b);
        }
    }
}
