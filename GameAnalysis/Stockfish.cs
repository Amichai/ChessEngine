using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using ChessKit.ChessLogic;
using ChessKit.ChessLogic.Algorithms;
using ChessKit.ChessLogic.Primitives;
using RunProcess;

namespace GameAnalysis
{
    internal sealed class Stockfish
    {
        private const string STOCKFISH_PATH = @"..\..\..\stockfish\8\stockfish_8_x64.exe";

        private readonly ProcessHost process;

        public Stockfish()
        {
            process = new ProcessHost(STOCKFISH_PATH, null);
            process.Start();
            SetPVCount(5);
            process.StdIn.WriteLine(Encoding.ASCII, "uci");
            string output = process.StdOut.ReadAllText(Encoding.ASCII);
            Debug.Print(output);
            //process.StdIn.WriteLine(Encoding.ASCII, "debug on");
            output = process.StdOut.ReadAllText(Encoding.ASCII);
            Debug.Print(output);
        }


        private int pvCount = 1;
        public void SetPVCount(int count)
        {
            process.StdIn.WriteLine(Encoding.ASCII, $"setoption name MultiPV value {count}");
            pvCount = count;
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

        private Dictionary<string, List<CandidateMove>> cached = new Dictionary<string, List<CandidateMove>>();

        public List<CandidateMove> CandidateMoves(Position pos, int? depth = null)
        {
            var position = pos.PrintFen();

            if (cached.ContainsKey(position))
            {
                return cached[position];
            }

            WaitUntilReady();
            process.StdIn.WriteLine(Encoding.ASCII, "ucinewgame");
            WaitUntilReady();

            process.StdIn.WriteLine(Encoding.ASCII, "position fen " + position);
            process.StdIn.WriteLine(Encoding.ASCII, $"go depth {depth ?? Depth}");
            var output = WaitForMove();
            var outputLines = output.Split('\n');

            var toReturn = new List<CandidateMove>();
            for (var i = 1; i <= pvCount; i++)
            {
                var inspectionLine = outputLines.LastOrDefault(j => j.StartsWith("info") && j.Contains($"multipv {i}"));
                if (inspectionLine == null)
                {
                    break;
                }

                toReturn.Add(CandidateMove.Parse(inspectionLine, pos.ActiveColor() == "b"));
            }

            cached[position] = toReturn;

            return toReturn;
        }

        public CandidateMove Bestmove(Position pos)
        {
            return CandidateMoves(pos).First();
        }

        public int Depth { get; set; } = 10;

        private Dictionary<string, double> evalCache = new Dictionary<string, double>();

        public double AnalyzePosition(Position positionState, out int mate)
        {
            var position = positionState.PrintFen();
            if (evalCache.ContainsKey(position))
            {
                mate = -1;
                return evalCache[position];
            }

            WaitUntilReady();
            process.StdIn.WriteLine(Encoding.ASCII, "ucinewgame");
            WaitUntilReady();

            process.StdIn.WriteLine(Encoding.ASCII, "position fen " + position);
            process.StdIn.WriteLine(Encoding.ASCII, $"go depth {Depth}");
            var output = WaitForMove();
            var outputLines = output.Split('\n');

            //var error = process.StdErr.ReadAllText(Encoding.ASCII);

            var inspectionLine = outputLines.Last(i => i.StartsWith("info") && i.Contains("multipv 1"));

            var eval = int.Parse(inspectionLine.Split(' ')[9]) / 100.0;

            //var move = outputLines.Last(i => i.StartsWith("bestmove")).Split(' ')[1];
            mate = -1;
            var parts = inspectionLine.Split(' ');
            var index = parts.IndexOf("mate");
            if (index != -1)
            {
                mate = int.Parse(parts[index + 1]);
            }

            var toReturn = eval * (positionState.Core.Turn == Color.White ? 1 : -1);

            evalCache[position] = toReturn;

            return toReturn;
        }
    }
}