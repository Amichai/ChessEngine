using System.Collections.Generic;
using System.Linq;
using ChessKit.ChessLogic;

namespace ChessEngine
{
    public class PositionAnalysis
    {
         private readonly Dictionary<LegalMove, double> positionVal = new Dictionary<LegalMove, double>();

        public double this[LegalMove move]
        {
            get { return positionVal[move]; }
            set { positionVal[move] = value; }
        }

        public List<LegalMove> Keys
        {
            get { return this.positionVal.OrderByDescending(i => i.Value).Select(i => i.Key).ToList(); }
        }
    }
}