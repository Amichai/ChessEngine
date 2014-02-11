using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine {
    public static class MiniMax {
        public static SingleMove Evaluate(BoardState startingPosition, 
            IBoardEvaluator eval,
            Func<BoardState, IEnumerable<BoardMove>> evolution,
            SideColor sideToMaximize){

                Dictionary<SingleMove, double> evals = new Dictionary<SingleMove, double>();
                foreach (var s in evolution(startingPosition)) {
                    evals[s.Move] = eval.Eval(s.Board);
                }

                return evals.OrderBy(i => i.Value).First().Key;
        }
    }
}
