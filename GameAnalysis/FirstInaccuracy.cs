using System.Diagnostics;
using System.Linq;
using ChessKit.ChessLogic;
using ChessKit.ChessLogic.Algorithms;

namespace GameAnalysis
{
    internal sealed class FirstInaccuracy
    {
        private readonly string _path;

        public FirstInaccuracy(string path)
        {
            _path = path;
        }

        private const double InaccuracyThreshold = .3;

        public void Process()
        {
            var stockfish = new Stockfish();
            stockfish.Depth = 18;

            var gp =  new GameProvider(_path);

            foreach (var game in gp.Games())
            {

                var properties = game.Properties;
                var white = properties["White"];
                var black = properties["Black"];

                var isWhite = white == "amichai";

                Debug.Print($"Is white: {isWhite} \n{properties["Site"]}");
                var evalPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1".ParseFen();
                double targetEval = 0;

                foreach (var position in game.Positions())
                {
                    var moves = stockfish.CandidateMoves(evalPosition);
                    var move = moves.First();
                    //var targetEval = move.Eval.Value;
                    //if (evalPosition.ActiveColor() == "b")
                    //{
                    //    targetEval *= -1;
                    //}

                    int mate;
                    var eval = stockfish.AnalyzePosition(position, out mate);


                    Debug.Print($"Target: {move.Move}, played: {position.Move.Move}, {eval}");

                    var activeColor = position.ActiveColor();
                    var criticalSide = (isWhite && activeColor == "b") || (!isWhite && activeColor == "w");

                    bool isInaccuracy = (activeColor == "b" && targetEval - eval >= InaccuracyThreshold) ||
                                        (activeColor == "w" && eval - targetEval >= InaccuracyThreshold);

                    if (isInaccuracy && criticalSide && move.Move != position.Move.Move.ToString().Replace("-", ""))
                    {
                        Debug.Print($"{targetEval}, {eval}, {position.Move.Move}");
                        Debug.Print(evalPosition.PrintFen());
                        break;
                    }

                    targetEval = eval;

                    evalPosition = position;

                }
            }
        }
    }
}