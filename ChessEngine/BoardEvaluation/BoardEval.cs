using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine.BoardEvaluation {
    public class BoardEval : IBoardEvaluator {
        public BoardEval() {
            this.pieceValues = new Dictionary<PieceType, double>() {
                {PieceType.Pawn, 1},
                {PieceType.Knight, 3},
                {PieceType.Bishop, 3.5},
                {PieceType.Rook, 5},
                {PieceType.Queen, 9},
                {PieceType.King, 0},
            };
        }
        private Dictionary<PieceType, double> pieceValues;
        public double Eval(BoardState s) {
            var pieces = s.OnTheBoard();
            double w = 0;
            foreach (var p in pieces[SideColor.White].Keys) {
                w += pieces[SideColor.White][p] * this.pieceValues[p];
            }
            double b = 0;
            foreach (var p in pieces[SideColor.Black].Keys) {
                b += pieces[SideColor.Black][p] * this.pieceValues[p];
            }
            return w - b;
        }
    }
}
