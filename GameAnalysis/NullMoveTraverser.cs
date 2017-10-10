using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChessKit.ChessLogic;
using ChessKit.ChessLogic.Algorithms;
using ChessKit.ChessLogic.Primitives;

namespace GameAnalysis
{
    internal class NullMoveTraverser
    {
        private readonly Stockfish _stockfish;

        private const double TargetEvalTolerance = .4;

        public NullMoveTraverser(Stockfish stockfish)
        {
            _stockfish = stockfish;
        }

        private List<CandidateMove> CandidateMoves(Position position)
        {
            var moves = _stockfish.CandidateMoves(position, 10);
            var targetEval = moves.First().Eval;

            var candidateMoves = moves.Where(i => targetEval - i.Eval < TargetEvalTolerance);

            return candidateMoves.ToList();
        }

        public void Traverse(Position position, PositionSet positions, double evalTarget, ref int depth, PlayedMoves playedMoves)
        {
            var activeColor = position.ActiveColor();
            var nullMoveTarget = activeColor == "w" ? evalTarget - 1 : evalTarget + 1;

            var nullMove = position.SwitchActiveColor();
            if ((position.Properties & GameStates.Check) == 0)
            {
                ///leafs, 1 CP improvement over eval target
                var c = _stockfish.CandidateMoves(nullMove, 10).ToList();
                List<CandidateMove> candidates1;
                if (activeColor == "b")
                {
                    candidates1 = c.Where(i => i.Eval > nullMoveTarget).ToList();
                }
                else
                {
                    candidates1 = c.Where(i => i.Eval < nullMoveTarget).ToList();
                }

                foreach (var candidate in candidates1)
                {
                    var newPos = nullMove.MakeMove2(candidate.Move);
                    Debug.Print($"null pos ({candidate.Eval}), {candidate.Move}:");
                    Debug.Print($"{nullMove.PrintFen()}");
                    var m = Move.Parse(candidate.Move.Insert(2, "-"));
                    playedMoves.Add(new MoveAndPosition(m, position));
                    positions.Add(new TraversedPosition(newPos, playedMoves.Clone()));
                }
            }
            else
            {
                positions.MateCount++;
            }

            IEnumerable<CandidateMove> candidates2;
            var c1 = _stockfish.CandidateMoves(position, 15);
            IEnumerable<CandidateMove> allCandidates;

            if (activeColor == "w")
            {
                allCandidates = c1.OrderByDescending(i => i.Eval);
                if (allCandidates.Any())
                {
                    evalTarget = allCandidates.Max(i => i.Eval).Value;
                }

                candidates2 = allCandidates.Where(i => i.Eval > evalTarget - TargetEvalTolerance);
            }
            else
            {
                allCandidates = c1.OrderBy(i => i.Eval);
                if (allCandidates.Any())
                {
                    evalTarget = allCandidates.Min(i => i.Eval).Value;
                }

               candidates2 = allCandidates.Where(i => i.Eval < evalTarget + TargetEvalTolerance);
            }

            ///add to positions
            foreach (var candidate in candidates2)
            {
                var newPosition = position.MakeMove2(candidate.Move);
                depth++;
                Debug.Print($"traverse pos ({candidate.Eval}):");
                Debug.Print($"{newPosition.PrintFen()}");
                var m = Move.Parse(candidate.Move.Insert(2, "-"));
                playedMoves.Add(new MoveAndPosition(m, position));
                if (depth < 5)
                {
                    Traverse(newPosition, positions, evalTarget, ref depth, playedMoves.Clone());
                }

                positions.Add(new TraversedPosition(newPosition, playedMoves));
                depth--;
                Debug.Print($"{positions.Positions.Count}, {depth}");
            }
        }

        //public void Traverse(Position startPosition, PositionSet positions)
        //{
        //    var active = startPosition.ActiveColor();
        //    var nullMove = startPosition.SwitchActiveColor();

        //    var candidates = CandidateMoves(nullMove);

        //    ///1 - traverse n - best moves, eval (within tolerance)
        //    /// For each move, play null move, return to step 1
        //    /// For each move, play n best moves,
        //    ///
        //    ///

        //    var candidateMoves = CandidateMoves(startPosition);



        //    throw new Exception();
        //}
    }
}
