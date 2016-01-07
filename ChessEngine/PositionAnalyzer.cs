using System.Collections.Generic;
using System.Threading.Tasks;
using ChessKit.ChessLogic;

namespace ChessEngine
{
    public class PositionAnalyzer
    {
        private class analysis
        {
            private readonly Position _position;
            private bool isCanceled = false;

            public analysis(Position position)
            {
                _position = position;
            }

            public Task<PositionAnalysis> Start()
            {
                var t = Task.Run(() =>
                {
                    var fen = Fen.Print(this._position);
                    var positionClone = Fen.Parse(fen);
                    var legalMoves = GetLegalMoves.All(positionClone);
                    var stockfish = new Stockfish();
                    var moveEval = new PositionAnalysis();
                    foreach (var move in legalMoves)
                    {
                        if (isCanceled)
                        {
                            break;
                        }
                        var newPosition = move.ToPosition();
                        var eval = stockfish.AnalyzePosition(newPosition);
                        moveEval[move] = eval.Eval * -1;
                    }
                    return moveEval;
                });
                return t;
            }

            public void Stop()
            {
                isCanceled = true;
            }
        }

        private analysis currentAnalysis = null;

        public Task<PositionAnalysis> Analyze(Position position)
        {
            var a = new analysis(position);
            if (currentAnalysis != null)
            {
                currentAnalysis.Stop();
            }
            currentAnalysis = a;
            return a.Start();
        }
    }
}