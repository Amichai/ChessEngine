using System.Collections.Generic;
using System.Linq;

namespace ChessGameBrowser.Web.Models
{
    public sealed class TargetMoves
    {
        public TargetMoves(params TargetMove[] moves)
        {
            this.Moves = moves.ToList();
        }

        public List<TargetMove> Moves { get; }
    }
}