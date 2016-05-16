using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using ChessKit.ChessLogic;

namespace BoardStateFormatter
{
    internal sealed class NotationConverter
    {
        public void Convert(string s)
        {
            var moves = s.Split(' ').Select(i => i.Trim()).ToList();

            var currentPosition = Fen.StartingPosition;

            foreach (var move in moves)
            {
                var moveLocal = move;
                if (char.IsDigit(move.First()))
                {
                    moveLocal = move.Split('.').Last();
                }
                var m = San.Parse(moveLocal, currentPosition);
                var legal = currentPosition.ValidateLegalMove(m.Move);
                var a = legal.ToPosition();
                currentPosition = a;

                Debug.Print("\"{0}\",", Fen.Print(a).Split(' ').First());
            }
        }
    }
}