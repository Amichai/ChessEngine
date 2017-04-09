using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChessKit.ChessLogic;
using ChessKit.ChessLogic.Algorithms;

namespace ChessKitTester
{
    internal sealed class BoardModel
    {
        private Position position;

        public BoardModel()
        {
            position = Fen.ParseFen(@"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");


        }

        public List<LegalMove> LegalMoves(int startSquare)
        {
            var x = startSquare % 8;
            var y = startSquare / 8;

            var legalMoves = position.GetLegalMovesFromSquare(x * 16 + y);
            return legalMoves;
        }

        public void Test()
        {
            for (var i = 0; i < 128; i++)
            {
                var legalMoves = position.GetLegalMovesFromSquare(i);
                //legalMoves.First().Move.
                //Debug.Print(i + ": " + legalMoves.Count.ToString());
            }

        }
    }
}