using System;
using System.Collections.Generic;
using ChessKit.ChessLogic;
using ChessKit.ChessLogic.Primitives;

namespace GameAnalysis
{
    internal static class Util
    {
        public static Position ApplyMove(this Position position, LegalMove move)
        {
            return new Position(move.ResultPosition, 0, position.MoveNumber + 1, GameStates.None, move);
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
    }
}