using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChessKit.ChessLogic;
using ChessKit.ChessLogic.Algorithms;
using ChessKit.ChessLogic.Primitives;

namespace GameAnalysis
{
    internal sealed class BoardAnalysis
    {
        private static readonly Stockfish stockfish = new Stockfish();

        public static void FindTactics(Position position)
        {
            int mate;
            var p = stockfish.AnalyzePosition(position, out mate);

            if (mate > 2)
            {
                Debug.Print($"Mate:\n{position.PrintFen()}");

                return;
            }

            var legal = position.GetAllLegalMoves();
            var evals = legal.Select(move => stockfish.AnalyzePosition(position.ApplyMove(move), out mate)).ToList();

            List<double> ordered;


            if (position.Core.Turn == Color.White)
            {
                ordered = evals.OrderByDescending(i => i).ToList();
            }
            else
            {
                ordered = evals.OrderBy(i => i).ToList();
            }

            var isWinning = (position.Core.Turn == Color.White && ordered[0] > 0) ||
                             (position.Core.Turn == Color.Black && ordered[0] < 0);

            if (ordered.Count > 1 && Math.Abs(ordered[0] - ordered[1]) >= 1.1 && isWinning)
            {
                Debug.Print($"Tactic:\n{position.PrintFen()}");
            }
        }

        private static IEnumerable<string> GetAdjustedFENs(string fen)
        {
            var c = ' ';

            for (var i = 0; i < fen.Length; i++)
            {
                var a = fen[i];

                if (a == '/' || char.IsDigit(a) || a == 'k' || a == 'K')
                {
                    continue;
                }

                if (a == ' ')
                {
                    yield break;
                }

                var b = fen[i + 1];

                if (i > 0)
                {
                    c = fen[i - 1];
                }

                if (char.IsDigit(b) && char.IsDigit(c))
                {
                    var s = int.Parse(b.ToString()) + int.Parse(c.ToString()) + 1;
                    yield return fen.Substring(0, i - 1) + s + fen.Substring(i + 2);
                }
                else if (char.IsDigit(b))
                {
                    var s = int.Parse(b.ToString()) + 1;
                    yield return fen.Substring(0, i) + s + fen.Substring(i + 2);
                }
                else if (char.IsDigit(c))
                {
                    var s = int.Parse(c.ToString()) + 1;
                    yield return fen.Substring(0, i - 1) + s + fen.Substring(i + 1);
                }
                else
                {
                    yield return fen.Substring(0, i) + "1" + fen.Substring(i + 1);
                }
            }
        }

        public static Dictionary<int, double> DeterminePieceValues(Position position)
        {
            var pieceVals = new Dictionary<int, double>();

            int mate;
            var initialEval = stockfish.AnalyzePosition(position, out mate);

            if (mate != -1)
            {
                return pieceVals;
            }

            var originalCells = position.Core.GetCopyOfCells();

            //"rnbqkbnr/pppppppp/8/8/3P4/8/PPP1PPPP/RNBQKBNR b KQkq d3 0 1"
            var fen = Fen.PrintFen(position);

            foreach (var adjustedFeN in GetAdjustedFENs(fen))
            {
                Debug.Print(adjustedFeN);

                var adjustedPosition = Fen.ParseFen(adjustedFeN);

                var eval = stockfish.AnalyzePosition(adjustedPosition, out mate);

                var newCells = adjustedPosition.Core.GetCopyOfCells();

                var pieceVal = mate == -1 ? eval - initialEval : -1;

                for (var i = 0; i < 128; i++)
                {
                    if (originalCells[i] != newCells[i])
                    {
                        Debug.Print(i.ToString());

                        var a = i/16;
                        var b = i%16;
                        Debug.Print($"{a}, {b}");

                        pieceVals[i] = pieceVal;

                        break;
                    }
                }

                Debug.Print($"Eval diff: {pieceVal}");
            }

            return pieceVals;
            ;
        }
    }
}