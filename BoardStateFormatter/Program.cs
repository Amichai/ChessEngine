using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessKit.ChessLogic;

namespace BoardStateFormatter
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = "C:\\Users\\Amichai\\Desktop\\Writing\\hartwig2.pgn";
            new PgnConverter().Convert(path);


            var notation = @":|52P:36P|12p:28p|53P:37P|5b:26b|62N:45N|11p:19p|57N:42N|1n:18n|61B:25B|2b:38b|51P:43P|6n:12n|55P:47P|38b,45N:45b|45b,59Q:45Q|4k,7r:5r,6k|37P:29P|18n:35n|45Q:46Q|25B,35n:25n|25n,42N:25N|13p:21p|58B:44B|26b,44B:44b|44b,46Q:44Q|19p:27p|60K,63R:61R,62K|10p:18p|25N:40N|3q:17q|17q,44Q:17Q|8p,17Q:17p|50P:42P|5r:3r|61R:53R|3r:11r|40N:50N|0r:3r|56R:60R|27p,36P:36p|36p,43P:36P|12n:2n|53R:52R|17p:25p|50N:56N|2n:17n|56N:41N|17n:32n|62K:53K|18p:26p|52R:50R|14p:22p|22p,29P:22P|15p,22P:22p|41N:58N|26p:34p|53K:44K|6k:13k|60R:61R|11r:59r|59r,61R:59R|3r,59R:59r|44K:52K|59r:3r|52K:44K|32n:26n|49P:41P|13k:20k|34p,41P:34P|25p,34P:34p|50R:49R|3r:59r|49R:17R|20k:11k|58N:52N|11k:10k|17R:33R|59r:43r|44K:53K|26n,36P:36n|53K:60K|36n:19n|33R:32R|10k:18k|32R:0R|18k:27k|47P:39P|19n:29n|0R:5R|29n:44n|5R:6R|44n,54P:54n|60K:53K|39P,54n:39n|6R:3R|27k:36k|52N:46N|36k:37k|46N:52N|37k:38k|3R:2R|43r:45r";
            var moves = new PgnConverter().Parse(notation);

            //var toPrint = string.Join("|", moves);
            //Debug.Print(toPrint);

            //Debug.Assert(toPrint == notation);




            for (int i = 0; i < moves.Count; i++)
            {
                var currentPosition = new PositionModel(Fen.StartingPosition);

                for (int j = 0; j < i + 1; j++)
                {
                    currentPosition = currentPosition.Move(moves[j]);
                }

                Debug.Print("CURRENT:");
                Debug.Print(currentPosition.ToString());

                var legalMoves = LegalMoveProvider.LegalMoves(currentPosition);

                Debug.Print("LEGAL:");
                foreach (var legalMove in legalMoves)
                {
                    var legalPosition = currentPosition.Clone();
                    legalPosition = legalPosition.Move(legalMove);
                    Debug.Print(legalPosition.ToString());
                }

                ///White's move vs black's move
                ///Pawn captures
            }
        }
    }
}
