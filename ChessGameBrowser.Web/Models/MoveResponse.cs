namespace ChessGameBrowser.Web.Models
{
    public sealed class MoveResponse
    {
        public MoveResponse(TargetMoves moves, string fen, string computerMove)
        {
            Moves = moves;
            FEN = fen;
            IsValid = true;
            ComputerMove = computerMove;
        }

        public MoveResponse()
        {
            IsValid = false;
        }

        public bool IsValid { get; set; }
        public string FEN { get; }
        public string ComputerMove { get; }

        public static MoveResponse Invalid()
        {
            return new MoveResponse();
        }

        public TargetMoves Moves { get; set; }
    }
}