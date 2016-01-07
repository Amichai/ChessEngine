using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Subjects;
using System;
using System.Threading.Tasks;
using ChessKit.ChessLogic;
using System.Reactive.Linq;

namespace ChessEngine
{
    public class PositionAnalyzer
    {
        private class analysis
        {
            private readonly Position _position;
            private bool isCanceled = false;

            private double moveCount = 0;
            public Subject<double> Progress = new Subject<double>();

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
                    this.moveCount = legalMoves.Length;
                    double counter = 0;
                    foreach (var move in legalMoves)
                    {
                        if (isCanceled)
                        {
                            break;
                        }
                        var newPosition = move.ToPosition();
                        var eval = stockfish.AnalyzePosition(newPosition);
                        moveEval[move] = eval.Eval * -1;
                        counter++;
                        Progress.OnNext(counter * 100.0 / moveCount);
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

        public Subject<double> Progress = new Subject<double>();

        public Task<PositionAnalysis> Analyze(Position position)
        {
            var a = new analysis(position);
            if (currentAnalysis != null)
            {
                currentAnalysis.Stop();
            }
            currentAnalysis = a;
            a.Progress.Subscribe(Progress.OnNext);
           
            return a.Start();
        }
    }
}