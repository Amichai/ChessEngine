namespace ChessEngine
{
    public class AnalysisResult
    {
        public CellCoordinate Start { get; private set; }
        public CellCoordinate End { get; private set; }

        public AnalysisResult(double eval, CellCoordinate start, CellCoordinate end)
        {
            Start = start;
            End = end;
            this.Eval = eval;
        }

        public double Eval { get; private set; }
    }
}