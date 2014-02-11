using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine {
    public interface IBoardEvaluator {
        double Eval(BoardState s);
    }
}
