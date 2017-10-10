using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessKit.ChessLogic;
using ChessKit.ChessLogic.Algorithms;
using ChessKit.ChessLogic.Primitives;

namespace GameAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {

            //var first = new FirstInaccuracy(@"C:\Users\Amichai\Desktop\lichess_amichai_2016-09-22.pgn");
            //first.Process();

            //return;



            //BoardAnalysis.AnalyzePosition("5rk1/1prbqppp/p1n1pn2/3P4/8/PP2PN2/3QBPPP/R4RK1 w - - 0 17".ParseFen());
            //var p = "3rr1k1/1p1b1pp1/7p/1P1R4/8/2n1B2P/5PP1/R4BK1 w - - 1 0";

            ///Grandmaster prep1
            //var p = "r4rk1/p1nq1pp1/1p1pp2p/8/P2PR3/1QP2N2/1P3PPP/R5K1 b Qq - 0 1";


            //Crazy opening position
            var p = "r1bqkbnr/pp1p1ppp/4p3/2pP4/1n2P3/5N2/PPP2PPP/RNBQKB1R w KQkq - 1 5";

            var analyzer = new PositionAnalyzer(p);
            analyzer.Analyze();
            //BoardAnalysis.AnalyzePosition(p.ParseFen());


            //var gp = new GameProvider();
            //foreach (var g in gp.Games().Skip(0))
            //{
            //    foreach (var position in g.Positions())
            //    {


            //        //Debug.Print(position.PrintFen() + " " + position.MaterialDifference());
            //        var tactic = BoardAnalysis.FindTactics(position);
            //        {
            //            var md = tactic.Position.MaterialDifference();
            //            var eval = tactic.Eval.Value;
            //            Debug.Print($"diff: {md}, {eval}");
            //            var diff = Math.Abs(md - eval);
            //            if (diff > 1.1)
            //            {
            //                var i = 0;
            //                BoardAnalysis.PlayForward(position, eval, ref i);
            //            }
            //        }

            //        //var a = BoardAnalysis.DeterminePieceValues(position);

            //    }
            //}
        }
    }
}
