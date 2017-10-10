using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChessKit.ChessLogic;
using ChessKit.ChessLogic.Algorithms;
using ChessKit.ChessLogic.Primitives;

namespace GameAnalysis
{
    internal sealed class LineTraverser
    {
        public enum MoveSet { Optimal, TopN }

        private readonly Stockfish stockfish;
        private readonly int terminationDepth;
        private readonly MoveSet _whiteMoveSet;
        private readonly MoveSet _blackMoveSet;
        private readonly int _n;

        public LineTraverser(Stockfish stockfish, int terminationDepth, MoveSet whiteMoveSet, MoveSet blackMoveSet, int n)
        {
            this.stockfish = stockfish;
            this.terminationDepth = terminationDepth;
            _whiteMoveSet = whiteMoveSet;
            _blackMoveSet = blackMoveSet;
            _n = n;
        }

        public PositionSet BreadthFirstSearch(Position position)
        {
            var toVisit = new Queue<Position>();
            toVisit.Enqueue(position);

            double? initialEval = null;

            var positionSet = new PositionSet(position);

            var saturated = false;

            while (toVisit.Any())
            {
                ///If the new eval in an improvement
                /// and the material diff is also an improvemnt
                /// and it's my turn


                if (toVisit.Count > 1000)
                {
                    saturated = true;
                }

                Debug.Print($"{toVisit.Count()}");

                var pos = toVisit.Dequeue();

                positionSet.Add(new TraversedPosition(pos));

                var moves = stockfish.CandidateMoves(pos, 7).ToList();
                if (initialEval == null)
                {
                    initialEval = moves.First().Eval;
                }

                for (var i = 0; i < Math.Min(_n, moves.Count); i++)
                {
                    var move = moves[i];

                    var legal2 = pos.ValidateLegal(Move.Parse(move.Move.Insert(2, "-")));
                    var last = pos.ApplyMove(legal2);

                    if (!saturated)
                    {
                        toVisit.Enqueue(last);
                    }

                    positionSet.Add(new TraversedPosition(last));

                    foreach (var m in move.InspectionLine)
                    {

                        var legal = last.ValidateLegal(Move.Parse(m.Insert(2, "-")));
                        last = last.ApplyMove(legal);

                        positionSet.Add(new TraversedPosition(last));
                    }
                }

                //extract full lines
            }

            return positionSet;
        }

        public PositionSet Traverse(Position position)
        {
            var toReturn = new PositionSet(position.Clone());
            PlayForward(position.Clone(), toReturn, new PlayedMoves());
            return toReturn;
        }

        private void PlayForward(Position position, PositionSet positions, PlayedMoves playedMoves)
        {
            if (playedMoves.Count > terminationDepth)
            {
                return;
            }

            var curentState = position.PrintFen().ParseFen();

            var moves = stockfish.CandidateMoves(position);
            var moveSet = position.ActiveColor() == "w" ? _whiteMoveSet : _blackMoveSet;
            //var n = moveSet == MoveSet.Optimal ? 1 : _n;

            if (moveSet == MoveSet.TopN)
            {
                //moves = moves.Take(n).ToList();

                //moves = new [] { moves.Take(_n).ToList().Random() };
                moves = moves.Take(2).ToList();

            }
            else
            {
                var bestEval = moves.First().Eval.Value;
                moves = moves.Where(i => bestEval - i.Eval.Value < .3).Take(_n).ToList();
            }

            foreach (var move in moves)
            {
                var m = Move.Parse(move.Move.Insert(2, "-"));
                var legal = curentState.ValidateLegal(m);
                var newPosition = curentState.ApplyMove(legal);
                playedMoves.Add(new MoveAndPosition(m, curentState));
                positions.Add(new TraversedPosition(newPosition, playedMoves.Clone()));
                PlayForward(newPosition, positions, playedMoves);
                playedMoves.RemoveLast();
            }
        }
    }
}