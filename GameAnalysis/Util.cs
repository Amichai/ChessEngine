using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChessKit.ChessLogic;
using ChessKit.ChessLogic.Algorithms;
using ChessKit.ChessLogic.Primitives;

namespace GameAnalysis
{
    internal static class Util
    {
        public static Position ApplyMove(this Position position, LegalMove move)
        {
            return move.ToPosition();
        }

        public static Position ApplyMove(this Position position, string move)
        {
            Debug.Print($"{position.PrintFen()}\n{move}");

            return position.MakeMove(move);
        }

        public static Position MakeMove2(this Position pos, string move)
        {
            var legal2 = pos.ValidateLegal(Move.Parse(move.Insert(2, "-")));
            var position = pos.ApplyMove(legal2);
            return position;
        }

        public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (predicate == null) throw new ArgumentNullException("predicate");

            int retVal = 0;
            foreach (var item in items)
            {
                if (predicate(item)) return retVal;
                retVal++;
            }
            return -1;
        }

        public static int IndexOf<T>(this IEnumerable<T> items, T item)
        {
            return items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i));
        }

        public static Position SwitchActiveColor(this Position pos)
        {
            var s = pos.PrintFen();
            var parts = s.Split(' ');
            parts[1] = parts[1] == "w" ? "b" : "w";
            return string.Join(" ", parts).ParseFen();
        }

        public static string ActiveColor(this Position pos)
        {
            var v = pos.PrintFen().Split(' ')[1];
            return v;
            //switch (v)
            //{
            //    case "w":
            //        return Color.White;
            //    case "b":
            //        return Color.Black;
            //    default:
            //        throw new Exception();
            //}
        }

        public static int MaterialDifference(this Position position)
        {
            var fen = position.PrintFen();
            int sum = 0;

            foreach (var c in fen.Split(' ').First())
            {
                switch (c)
                {
                    case 'r':
                        sum -= 5;
                        break;
                    case 'p':
                        sum -= 1;
                        break;
                    case 'b':
                        sum -= 3;
                        break;
                    case 'n':
                        sum -= 3;
                        break;
                    case 'q':
                        sum -= 9;
                        break;

                    case 'R':
                        sum += 5;
                        break;
                    case 'P':
                        sum += 1;
                        break;
                    case 'B':
                        sum += 3;
                        break;
                    case 'N':
                        sum += 3;
                        break;
                    case 'Q':
                        sum += 9;
                        break;
                }
            }

            return sum;
        }

        public static Position Clone(this Position position)
        {
            return position.PrintFen().ParseFen();
        }

        public static List<BoardPiece> GetPieces(this Position position)
        {
            var pieces = new List<BoardPiece>();

            foreach (var square in Coordinates.All)
            {
                var piece = (Piece) position.Core.Cells[square];

                pieces.Add(new BoardPiece(piece, square.GetX(), square.GetY()));
            }

            return pieces;
        }

        private static Random rand = new Random();

        public static T Random<T>(this List<T> source)
        {
            var idx = rand.Next(source.Count);
            return source[idx];
        }
    }
}