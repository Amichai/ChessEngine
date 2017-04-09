using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ChessKit.ChessLogic;
using ChessKit.ChessLogic.Algorithms;
using ChessKit.ChessLogic.Primitives;

namespace GameAnalysis
{
    public sealed class GameProvider
    {

        private const string PATH = @"C:\Users\Amichai\Data\millionbase-2.22.pgn";

        private IEnumerable<string> lines;

        public GameProvider(string path = null)
        {
            path = path ?? PATH;

            lines = File.ReadLines(path);
        }

        public IEnumerable<Game> Games()
        {
            var sb = new StringBuilder();

            var kv = new Dictionary<string, string>();

            foreach (var line in lines)
            {
                if (line.Length > 0 && line.First() == '[')
                {
                    var l = line.TrimStart('[').TrimEnd(']');
                    var key = l.Split(' ').First();
                    var value = l.Substring(key.Length + 1);
                    kv[key] = value;
                }

                if (line.Length == 0 || !char.IsDigit(line.First()))
                {
                    continue;
                }

                if (line.Length > 1 && line.Substring(0, 2) == "1." && sb.Length > 0)
                {
                    yield return new Game(sb.ToString(), kv);

                    sb.Clear();

                    kv = new Dictionary<string, string>();
                }

                sb.Append(line + " ");
            }
        }
    }
}