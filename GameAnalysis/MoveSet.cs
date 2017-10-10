using System.Collections.Generic;
using System.Linq;
using ChessKit.ChessLogic;

namespace GameAnalysis
{
    internal sealed class PlayedMoves
    {
        public int Count => Moves.Count;

        public List<MoveAndPosition> Moves { get; }

        public PlayedMoves()
        {
            Moves = new List<MoveAndPosition>();
        }

        public PlayedMoves(List<MoveAndPosition> moves)
        {
            this.Moves = moves;
        }

        public void Add(MoveAndPosition move)
        {
            Moves.Add(move);
        }

        public PlayedMoves Clone()
        {
            return new PlayedMoves(Moves.ToList());
        }

        public void RemoveLast()
        {
            Moves.RemoveAt(Moves.Count - 1);
        }
    }
}