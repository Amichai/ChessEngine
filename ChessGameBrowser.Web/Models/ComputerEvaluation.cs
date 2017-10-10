using ChessKit.ChessLogic;

namespace ChessGameBrowser.Web.Models
{
    internal sealed class ComputerEvaluation
    {
        private static readonly Stockfish stockfish = new Stockfish();

        public static string Eval(string position, out string bestLine)
        {
            int mate;
            var val = stockfish.AnalyzePosition(Fen.Parse(position), out mate, out bestLine);

            if (mate != -1)
            {
                return "#" + mate;
            }

            return val.ToString();
        }
    }
}