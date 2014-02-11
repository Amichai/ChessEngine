using ChessEngine.BoardEvaluation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine {
    public class BoardAnalyzer {
        private Subject<BoardState> newPosition;
        private MoveList moveList;
        public Subject<SingleMove> BestMove;
        public BoardAnalyzer(Subject<BoardState> newPosition,
            MoveList moveList) {
            this.newPosition = newPosition;
            this.BestMove = new Subject<SingleMove>();
            this.moveList = moveList;
            this.newPosition.Subscribe(i => {
                var eval = new BoardEval();
                var resolved = MiniMax.Evaluate(i, eval,
                    s => boardEvolver(s, SideColor.Black), 
                    SideColor.White);
                this.BestMove.OnNext(resolved);
                Debug.Print(resolved.ToString());
                //foreach (var s in boardEvolver(i, SideColor.White)) {
                //    Debug.Print(s.ToString());
                //    Debug.Print("\n");
                //}
            });
        }

        private IEnumerable<BoardMove> boardEvolver(BoardState board, SideColor turn) {
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    var p = board.State[i][j];
                    if (p != null && p.Color == turn) {
                        var available = PieceExtensions.GetAvailableCells(i, j, p, board.State, this.moveList);
                        foreach (var a in available) {
                            yield return board.ApplyMove(i, j, a, p);
                        }
                    }
                }
            }
        }

        ///Observe each new move
        ///Build a component for listing all possible moves given a configuration
        ///Evaluate each configuration
    }

    public class BoardMove {
        public BoardState Board { get; set; }
        public SingleMove Move { get; set; }
    }
}
