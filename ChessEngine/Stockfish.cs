using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChessKit.ChessLogic;
using RunProcess;

namespace ChessEngine
{
    public class Stockfish
    {
        private const string STOCKFISH_PATH = @"..\..\stockfish\stockfish-6-64.exe";

        private readonly ProcessHost process;

        public Stockfish()
        {
            process = new ProcessHost(STOCKFISH_PATH, null);
            process.Start();
            process.StdIn.WriteLine(Encoding.ASCII, "uci");
            string output = process.StdOut.ReadAllText(Encoding.ASCII);
            Debug.Print(output);
            //process.StdIn.WriteLine(Encoding.ASCII, "debug on");
            output = process.StdOut.ReadAllText(Encoding.ASCII);
            Debug.Print(output);
        }

        private void WaitUntilReady()
        {
            process.StdIn.WriteLine(Encoding.ASCII, "isready");
            while (true)
            {
                var result = process.StdOut.ReadAllText(Encoding.ASCII);
                var r = result.Split('\n');
                if (r.Any(i => i.Contains("readyok")))
                {
                    break;
                }
                Thread.Sleep(100);
            }
        }

        private string WaitForMove()
        {
            while (true)
            {
                var result = process.StdOut.ReadAllText(Encoding.ASCII);
                var r = result.Split('\n');
                if (r.Any(i => i.Contains("bestmove")))
                {
                    return result;
                }
                Thread.Sleep(100);
            }
        }

        public AnalysisResult AnalyzePosition(Position positionState)
        {
            string position = Fen.Print(positionState);
            WaitUntilReady();
            process.StdIn.WriteLine(Encoding.ASCII, "ucinewgame");
            WaitUntilReady();

            process.StdIn.WriteLine(Encoding.ASCII, "position fen " + position);
            process.StdIn.WriteLine(Encoding.ASCII, "go depth 10");
            var output = WaitForMove();
            var outputLines = output.Split('\n');
            var lastLine = outputLines.Last(i => i.StartsWith("info"));
            var error = process.StdErr.ReadAllText(Encoding.ASCII);
            Debug.Print("error: " + error);
            Debug.Print(output);
            var eval = int.Parse(lastLine.Split(' ')[9])/100.0;
            var move = outputLines.Last(i => i.StartsWith("bestmove")).Split(' ')[1];
            var start = CellCoordinate.FromString(move.Substring(0, 2));
            var end = CellCoordinate.FromString(move.Substring(2, 2));

            return new AnalysisResult(eval, start, end);
        }

    }
}