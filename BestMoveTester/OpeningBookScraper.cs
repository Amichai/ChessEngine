using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using ChessKit.ChessLogic;
using ChessKit.ChessLogic.Algorithms;
using GameAnalysis;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace BestMoveTester
{
    internal sealed class OpeningBookScraper
    {
        //private Dictionary<string, SubsequentPositions> positions = new Dictionary<string, SubsequentPositions>();

        private const string START_POSITION = @"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq";


        private ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

        private string ShortFEN(Position position)
        {
            var p = position.PrintFen();
            var parts = p.Split(' ');
            var abridged = parts.Take(parts.Length - 2);

            return string.Join(" ", abridged);
        }

        //private void Scrape1()
        //{
        //    var db = redis.GetDatabase();
        //    var r = db.StringGet(START_POSITION);

        //    var provider = new GameProvider();
        //    foreach (var game in provider.Games())
        //    {
        //        var currentPosition = START_POSITION;
        //        foreach (var nextPosition in game.Positions())
        //        {
        //            SubsequentPositions subsequentPositions;
        //            if (!positions.TryGetValue(currentPosition, out subsequentPositions))
        //            {
        //                subsequentPositions = new SubsequentPositions();
        //                positions[currentPosition] = subsequentPositions;
        //            }

        //            var nextPositionFen = ShortFEN(nextPosition);

        //            subsequentPositions.Add(nextPositionFen, game.GameResult);

        //            var val = subsequentPositions.ToString();


        //            db.StringSet(currentPosition, val);

        //            currentPosition = nextPositionFen;
        //        }

        //        Debug.Print($"{game.Id}");
        //    }
        //}

        private void Scrape2()
        {
            var db = redis.GetDatabase();
            var provider = new GameProvider();
            int lastGameId = 1454;
            foreach (var game in provider.Games())
            {
                if (game.Id <= lastGameId)
                {
                    continue;
                }

                var currentPosition = START_POSITION;
                foreach (var nextPosition in game.Positions())
                {
                    SubsequentPositions subsequentPositions;
                    var match = db.StringGet(currentPosition);

                    if (match.HasValue)
                    {
                        subsequentPositions = SubsequentPositions.FromString(match.ToString());
                    }
                    else
                    {
                        subsequentPositions = new SubsequentPositions();
                    }

                    var nextPositionFen = ShortFEN(nextPosition);

                    subsequentPositions.Add(nextPositionFen, game.GameResult);

                    var val = subsequentPositions.ToString();

                    db.StringSet(currentPosition, val);

                    currentPosition = nextPositionFen;
                }

                Debug.Print($"{game.Id}");
            }
        }

        public OpeningBookScraper()
        {
            Scrape2();
        }
    }

    class SubsequentPositions
    {
        public Dictionary<string, PositionStats> Positions { get; }

        public SubsequentPositions()
        {
            Positions = new Dictionary<string, PositionStats>();
        }

        public void Add(string position, int result)
        {
            PositionStats positionStats;
            if (!Positions.TryGetValue(position, out positionStats))
            {
                positionStats = new PositionStats();
                Positions[position] = positionStats;
            }

            positionStats.Add(result);
        }

        public override string ToString()
        {
            return string.Join(";", Positions.Select(i => $"{i.Key}:{i.Value}"));
        }


        //position1:w1,b2,d44;
        public static SubsequentPositions FromString(string s)
        {
            var toReturn = new SubsequentPositions();

            var positions = s.Split(';');
            foreach (var position in positions)
            {
                var parts = position.Split(':');
                toReturn.Positions[parts.First()] = PositionStats.FromString(parts.Last());
            }

            return toReturn;
        }
    }

    class PositionStats
    {
        public int White { get; private set; }
        public int Black { get; private set; }
        public int Draw { get; private set; }

        public void Add(int result)
        {
            switch (result)
            {
                case 1:
                    White++;
                    break;
                case -1:
                    Black++;
                    break;
                case 0:
                    Draw++;
                    break;
                default:
                    throw new Exception();
            }
        }

        public override string ToString()
        {
            return $"w{White},b{Black},d{Draw}";
        }

        public static PositionStats FromString(string val)
        {
            var parts = val.Split(',');
            var toReturn = new PositionStats();

            toReturn.White = int.Parse(parts[0].Substring(1));
            toReturn.Black = int.Parse(parts[1].Substring(1));
            toReturn.Draw = int.Parse(parts[2].Substring(1));

            return toReturn;

        }
    }
}