using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChessKit.ChessLogic;
using ilf.pgn;
using ilf.pgn.Data;

namespace BoardStateFormatter
{
    internal sealed class PgnConverter
    {
        internal void Convert(string path)
        {
            var reader = new PgnReader();
            var db = reader.ReadFromFile(path);

            foreach (var game in db.Games)
            {

                Debug.Print("\"white\": \"{0}\", \"black\": \"{1}\", \"year\": {2},", game.WhitePlayer, game.BlackPlayer, game.Year);
                var diffs = new List<MoveModel>();
                diffs.Add(new MoveModel());
                var currentPosition = Fen.StartingPosition;
                var models = new List<PositionModel>();
                models.Add(new PositionModel(currentPosition));

                var moves = game.MoveText.GetMoves();

                foreach (var move in moves)
                {
                    var m = San.Parse(move.ToString(), currentPosition);
                    var legal = currentPosition.ValidateLegalMove(m.Move);
                    var a = legal.ToPosition();
                    currentPosition = a;

                    var positionModel = new PositionModel(a);

                    models.Add(positionModel);

                    //Debug.Print("\"{0}\",", positionModel);
                }

                int moveCount = 0;

                for (int i = 0; i < models.Count - 1; i++)
                {
                    var diff = models[i].Diff(models[i + 1]);

                    diffs.Add(diff);
                }

                var comments = Enumerable.Range(0, diffs.Count).Select(i => string.Empty).ToList();

                foreach (var move in game.MoveText)
                {
                    switch (move.Type)
                    {
                        case MoveTextEntryType.MovePair:
                            moveCount += 2;
                            break;
                        case MoveTextEntryType.SingleMove:
                            moveCount++;
                            break;
                        case MoveTextEntryType.GameEnd:
                            break;
                        case MoveTextEntryType.Comment:
                            var c = move.ToString().TrimStart('{').TrimEnd('}');
                            comments[moveCount] = c;
                            break;
                        case MoveTextEntryType.NumericAnnotationGlyph:
                            break;
                        case MoveTextEntryType.RecursiveAnnotationVariation:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                Debug.Print($"\"moves\":\"{string.Join("|", diffs)}\",");

                Debug.Print(
                    $"\"annotations\": [{string.Join("", comments.Select(i => { var toReturn = "\"" + i.Replace("\"", "\\\"") + "\","; if (i.Length > 0) { toReturn += "\n"; } return toReturn; }))}]");
            }
        }

        internal List<MoveModel> Parse(string gameNotation)
        {
            var position = Fen.StartingPosition;
            var model = new PositionModel(position);

            var moves = gameNotation.Split('|');

            return moves.Select(i => MoveModel.Parse(i)).ToList();
        }
    }
}