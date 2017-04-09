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
            //BoardAnalysis.FindTactics(Fen.ParseFen("4Nb2/R7/7p/3Ppkp1/1r6/8/5P1K/8 w - - 7 57"));

            var gp = new GameProvider();
            foreach (var g in gp.Games().Skip(0))
            {
                foreach (var position in g.Positions())
                {
                    Debug.Print(position.PrintFen());
                    //BoardAnalysis.FindTactics(position);

                    var a = BoardAnalysis.DeterminePieceValues(position);

                }
            }
        }
    }
}
