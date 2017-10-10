using System.IO;
using CChessCore.Pgn;
using ChessKit.ChessLogic.Algorithms;

namespace BestMoveTester
{
    internal sealed class GameDB
    {
        public GameDB()
        {
            var path = @"C:\Users\Amichai\Data\millionbase-2.22.pgn.pgn";

            var reader = new StringReader(File.ReadAllText(path));
            var _pgnReader = new PgnReader(reader, PgnReader.DefaultBufferSize);

            _pgnReader.ReadGame();
            var game = _pgnReader.CurrentGame;


            var position = START_POSITION.ParseFen();


        }

        private const string START_POSITION = @"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    }
}