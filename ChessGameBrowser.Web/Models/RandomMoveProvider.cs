using System;
using System.Collections.Generic;
using System.Linq;
using ChessKit.ChessLogic;
using ChessKit.ChessLogic.Algorithms;

namespace ChessGameBrowser.Web.Models
{
    internal sealed class RandomMoveProvider : ITargetMoveProvider
    {
        private static readonly Random random = new Random();

        public TargetMoves Get(Position position)
        {
            var legal = position.GetAllLegalMoves();

            var indices = new HashSet<int>();

            for (var i = 0; i < 4; i++)
            {
                var idx = random.Next(legal.Count);

                indices.Add(idx);
            }

            return new TargetMoves(indices.Select(i =>
            {
                var move = legal[i];


                var white = random.Next(1000);
                var black = random.Next(1000);
                var draw = random.Next(1000);

                return new TargetMove(move.ResultPosition, white, black, draw, move.Move.ToString());
            }).ToArray());

        }
    }
}