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

        public static int AssessDepth(ResolvedTactic tactic)
        {
            throw new Exception();
        }

        public static void PlayForward(Position position, double eval, List<TraversedPosition> positions, ref int depth)
        {
            if (depth > 5)
            {
                return;
            }

            var curentState = position.PrintFen().ParseFen();

            var moves = stockfish.CandidateMoves(position);

            if (depth % 2 == 1)
            {
                moves = moves.Take(1).ToList();
            }

            //Debug.Print($"Move count: {moves.Count}");
            foreach (var move in moves)
            {
                var legal = curentState.ValidateLegal(Move.Parse(move.Move.Insert(2, "-")));
                var newPosition = curentState.ApplyMove(legal);
                positions.Add(new TraversedPosition(newPosition, depth));
                //var diff = eval - newPosition.MaterialDifference();
                //Debug.Print($"Move: {move}, diff: {diff}, - {depth}");
                depth++;
                PlayForward(newPosition, eval, positions, ref depth);
                depth--;
            }
        }

        public static ResolvedTactic FindTactics(Position position)
        {
            int mate;
            var p = stockfish.AnalyzePosition(position, out mate);

            if (mate > 2)
            {
                Debug.Print($"Mate:\n{position.PrintFen()}");

                return new ResolvedTactic(position, null);
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

            Debug.Print($"{ordered.First()}");

            if (ordered.Count > 1 && Math.Abs(ordered[0] - ordered[1]) >= 1.1 && isWinning)
            {
                Debug.Print($"Tactic:\n{position.PrintFen()}");

                return new ResolvedTactic(position, ordered[0]);
            }

            return null;
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
        }

        public static int TacticComplexity(ResolvedTactic tactic)
        {
            var md = tactic.Position.MaterialDifference();
            var eval = tactic.Eval.Value;
            Debug.Print($"diff: {md}, {eval}");
            var diff = Math.Abs(md - eval);
            var positions = new List<TraversedPosition>();
            if (diff > 1.1)
            {
                var i = 0;
                PlayForward(tactic.Position, eval, positions, ref i);
            }

            //var resolved = positions.Where(i => i.Position.MaterialDifference() - md > 1).ToList();
            List<TraversedPosition> resolved;
            if (tactic.Position.ActiveColor() == "b")
            {
                resolved = positions.Where(i => md - i.Position.MaterialDifference() > 1.1).ToList();
            }
            else
            {
                resolved = positions.Where(i => i.Position.MaterialDifference() - md > 1.1).ToList();
            }


            return resolved.Select(i => i.Depth).Min();
        }

        public static void AnalyzePosition(Position position)
        {

            var tactic = FindTactics(position);
            var complexity = TacticComplexity(tactic);

            Debug.Print($"Tactic complexity: {complexity}");
            //var md = tactic.Position.MaterialDifference();
            //var eval = tactic.Eval.Value;
            //Debug.Print($"diff: {md}, {eval}");
            //var diff = Math.Abs(md - eval);
            //if (diff > 1.1)
            //{
            //    var i = 0;
            //    PlayForward(position, eval, ref i);
            //}
        }
    }
}