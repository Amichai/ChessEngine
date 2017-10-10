using ChessKit.ChessLogic;

namespace GameAnalysis
{
    internal sealed class ResolvedTactic
    {
        public Position Position { get; }
        public double? Eval;

        public ResolvedTactic(Position position, double? eval)
        {
            Position = position;
            Eval = eval;
        }
    }
}