using System.Collections.Generic;
using ChessKit.ChessLogic;
using ChessKit.ChessLogic.Algorithms;

namespace GameAnalysis
{
    internal sealed class PositionSet
    {
        private readonly Position _initialPosition;

        private HashSet<string> visited = new HashSet<string>();

        public List<TraversedPosition> Positions { get; }

        public int MateCount { get; set; }

        public PositionSet(Position initialPosition)
        {
            Positions = new List<TraversedPosition>();
            _initialPosition = initialPosition;
        }

        public void Add(TraversedPosition position)
        {
            var fen = position.Position.PrintFen();
            if (visited.Contains(fen))
            {
                return;
            }

            Positions.Add(position);

            visited.Add(fen);
        }
    }
}