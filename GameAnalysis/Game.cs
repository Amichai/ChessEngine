using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using ChessKit.ChessLogic;
using ChessKit.ChessLogic.Algorithms;
using ChessKit.ChessLogic.Primitives;

namespace GameAnalysis
{
    public sealed class Game
    {
        private static int IdCounter = 0;
        private readonly List<string> moves = new List<string>();

        private const string START_POSITION = @"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        private float Result;

        public int GameResult { get; }

        public Dictionary<string, string> Properties { get; }

        public int Id { get; set; }

        public Game(string val, Dictionary<string, string> properties = null)
        {
            Id = IdCounter++;
            Properties = properties;
            var parts = val.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //Debug.Print($"Game:\n{val}");
            foreach (var part in parts)
            {
                if (char.IsDigit(part.First()))
                {
                    continue;
                }

                moves.Add(part);
            }

            switch (parts.Last())
            {
                case "0-1":
                    Result = -1;
                    GameResult = -1;
                    break;

                case "1-0":
                    Result = 1;
                    GameResult = 1;
                    break;

                case "1/2-1/2":
                    Result = .5f;
                    GameResult = 0;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public IEnumerable<Position> Positions()
        {
            var position = Fen.ParseFen(START_POSITION);

            foreach (var move in moves)
            {
                //var m = position.ParseMoveFromSan(move);

                //position = new Position(m.ResultPosition, 0, position.MoveNumber + 1, GameStates.None, m);

                position = position.MakeMove(move.TrimEnd('+'));

                yield return position;
            }
        }
    }
}