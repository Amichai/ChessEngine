using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using ChessKit.ChessLogic.Primitives;

namespace GameAnalysis
{
    internal class PositionEnsembleAnalysis
    {
        private readonly Dictionary<BoardPiece, List<PiecePositionCompareResult>> results = new Dictionary<BoardPiece, List<PiecePositionCompareResult>>();

        public PositionEnsembleAnalysis()
        {
        }

        public void Add(BoardPiece piece, PiecePositionCompareResult result)
        {
            List<PiecePositionCompareResult> compareResults;

            if (!results.TryGetValue(piece, out compareResults))
            {
                compareResults = new List<PiecePositionCompareResult>();
                results[piece] = compareResults;
            }

            compareResults.Add(result);
        }

        public void PrintStats()
        {
            foreach (var kv in results.OrderByDescending(i => i.Value.Count(j => j.CompareResult == PiecePositionCompareResult.CapturedInPlaceResultValue)))
            {
                Debug.Print($"{Enum.GetName(typeof(Piece), kv.Key.Piece)} - {kv.Key.X}, {kv.Key.Y}");
                Debug.Print($"Captured in place percentage: {kv.Value.Count(i => i.CompareResult == PiecePositionCompareResult.CapturedInPlaceResultValue) / (double) kv.Value.Count}");
                Debug.Print($"Captured in action percentage: {kv.Value.Count(i => i.CompareResult == PiecePositionCompareResult.CapturedInActionResultValue) / (double) kv.Value.Count}");
                Debug.Print($"Moved percentage: {kv.Value.Count(i => i.CompareResult == PiecePositionCompareResult.MovedResultValue) / (double) kv.Value.Count}");
                Debug.Print($"Survived percentage: {kv.Value.Count(i => i.CompareResult == PiecePositionCompareResult.SurvivedResultValue) / (double)kv.Value.Count}");
            }
        }
    }
}
