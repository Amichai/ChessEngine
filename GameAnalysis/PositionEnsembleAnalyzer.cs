using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChessKit.ChessLogic;
using ChessKit.ChessLogic.Algorithms;
using ChessKit.ChessLogic.Primitives;

namespace GameAnalysis
{
    internal sealed class PositionEnsembleAnalyzer
    {
        private readonly Position _initialPosition;
        private readonly PositionSet _traveresed;

        public PositionEnsembleAnalyzer(Position initialPosition, PositionSet traveresed)
        {
            _initialPosition = initialPosition;
            _traveresed = traveresed;
        }

        public PositionEnsembleAnalysis Analyze()
        {
            ///For each piece in the initial position
            /// What % did that piece move, stay, captured
            /// If it moved, where to?
            var pieces = _initialPosition.GetPieces();

            var result2 = new PositionEnsembleAnalysis();

            foreach (var piece in pieces.Where(i => i.Piece != Piece.EmptyCell))
            {
                foreach (var pos in _traveresed.Positions)
                {
                    var result = PiecePositionComparer.Compare(pos, piece);

                    result2.Add(piece, result);
                }
            }

            result2.PrintStats();
            return result2;
        }
    }
}