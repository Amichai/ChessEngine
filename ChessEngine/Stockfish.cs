using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            process.StdIn.WriteLine(Encoding.ASCII, "position startpos moves e2e4");
            string output = process.StdOut.ReadAllText(Encoding.ASCII);
            Debug.Print(output);
            process.StdIn.WriteLine(Encoding.ASCII, "go depth 10");
            output = process.StdOut.ReadAllText(Encoding.ASCII);
            Debug.Print(output);
        }

        public double AnalyzePosition(string position)
        {
            process.StdIn.WriteLine(Encoding.ASCII, "position fen " + position);
            process.StdIn.WriteLine(Encoding.ASCII, "go depth 10");
            var output = process.StdOut.ReadAllText(Encoding.ASCII);
            var outputLines = output.Split('\n');
            var lastLine = outputLines.Last(i => i.StartsWith("info"));
            var error = process.StdErr.ReadAllText(Encoding.ASCII);
            Debug.Print(error);
            Debug.Print(output);
            return int.Parse(lastLine.Split(' ')[9])/100.0;
        }

    }
}