using ChessKit.ChessLogic;

namespace ChessGameBrowser.Web.Models
{
    internal interface ITargetMoveProvider
    {
        TargetMoves Get(Position position);
    }
}