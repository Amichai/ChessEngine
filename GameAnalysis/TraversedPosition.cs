using System.Collections.Generic;
using ChessKit.ChessLogic;

namespace GameAnalysis
{
    internal sealed class TraversedPosition
    {
        public int Depth { get; }
        public Position Position { get; }
        public PlayedMoves Moves { get; }
        public int MaterialDifference { get; }

        public TraversedPosition(Position pos)
        {
            Position = pos;
            MaterialDifference = pos.MaterialDifference();
        }

        public TraversedPosition(Position pos, PlayedMoves moves) : this(pos)
        {
            Position = pos;
            Moves = moves;
            Depth = moves.Count;
        }

        public TraversedPosition(Position pos, int depth) : this(pos)
        {
            Position = pos;
            Depth = depth;
        }
    }
}