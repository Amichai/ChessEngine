using System.Diagnostics;
using System.Linq;
using ChessKit.ChessLogic;
using ChessKit.ChessLogic.Algorithms;
using ChessKit.ChessLogic.Primitives;

namespace GameAnalysis
{
    internal sealed class PositionAnalyzer
    {
        private readonly Position position;
        private readonly Stockfish stockfish;

        public PositionAnalyzer(string pos)
        {
            position = pos.ParseFen();
            stockfish = new Stockfish();
        }

        public void Analyze()
        {
            var color = position.ActiveColor();
            int mate;
            var eval = stockfish.AnalyzePosition(position, out mate);
            //var whiteMoveSet = color == "w" ? LineTraverser.MoveSet.Optimal : LineTraverser.MoveSet.TopN;
            //var blackMoveSet = color == "b" ? LineTraverser.MoveSet.Optimal : LineTraverser.MoveSet.TopN;


            var seen = new PositionSet(position);
            var traverser2 = new NullMoveTraverser(stockfish);

            Debug.Print($"Start position: {position.PrintFen()}");
            var depth = 0;
            PlayedMoves moves = new PlayedMoves();
            traverser2.Traverse(position, seen, eval, ref depth, moves);

            //var traverser = new LineTraverser(stockfish, 7, whiteMoveSet, blackMoveSet, 3);
            //var seen = traverser.Traverse(position);
            //var seen = traverser.BreadthFirstSearch(position);
            ///Iterate the game forward
            ///
            var ensemble = new PositionEnsembleAnalyzer(position, seen);
            ensemble.Analyze();
            ///
            //var diff = seen.Positions.Where(i => i.MaterialDifference != position.MaterialDifference()).ToList();
        }
    }
}