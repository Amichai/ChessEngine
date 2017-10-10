using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using ChessKit.ChessLogic;
using Newtonsoft.Json.Linq;

namespace ChessGameBrowser.Web.Models
{
    internal sealed class LiChessMoveProvider : ITargetMoveProvider
    {
        private Dictionary<string, string> CastleTransform = new Dictionary<string, string>()
        {
            {"e8-h8", "e8-g8" },
            {"e8-a8", "e8-c8" },
            {"e1-h1", "e1-g1" },
            {"e1-a1", "e1-c1" },
        };

        public TargetMoves Get(Position position)
        {
            var fen = Fen.Print(position);
            var bookMoves = GetBookMoves(fen);

            var moves = bookMoves.Moves.OrderByDescending(i => i.Black + i.White + i.Draws);

            return new TargetMoves(moves.Select(i =>
            {
                var pos = Fen.Parse(fen);
                var m2 = pos.ParseSanMove(i.San);
                pos = m2.ToPosition();

                var uci = i.Uci.Insert(2, "-");

                if (i.San.StartsWith("O-O") && CastleTransform.ContainsKey(uci))
                {
                    uci = CastleTransform[uci];
                }

                return new TargetMove(pos.Core, i.White, i.Black, i.Draws, uci, i.San);
            }).ToArray());
        }

        private BookMoves GetBookMoves(string position)
        {
            var p = position.Replace("/", "%2F").Replace(" ", "%20");

            string url = $@"https://expl.lichess.org/master?fen={p}&moves=12";

            var request = (HttpWebRequest) WebRequest.Create(url);
            using (var response = (HttpWebResponse)request.GetResponse()) {
                var resStream = response.GetResponseStream();
                var result = new StreamReader(resStream).ReadToEnd();

                var bookMoves = BookMoves.FromJson(result);
                return bookMoves;
            }
        }

        private class BookMoves
        {
            private BookMoves()
            {
                Moves = new List<BookMove>();
            }

            private int White { get; set; }
            private int Draws { get; set; }
            private int Black { get; set; }
            public List<BookMove> Moves { get; private set; }

            public static BookMoves FromJson(string json)
            {
                var toReturn = new BookMoves();
                var jobject = JObject.Parse(json);

                toReturn.White = jobject["white"].Value<int>();
                toReturn.Black = jobject["black"].Value<int>();
                toReturn.Draws = jobject["draws"].Value<int>();

                foreach (var move in jobject["moves"])
                {
                    toReturn.Moves.Add(BookMove.Parse(move));
                }

                return toReturn;
            }
        }

        private class BookMove
        {
            public string Uci { get; set; }
            public string San { get; set; }
            public int White { get; set; }
            public int Draws { get; set; }
            public int Black { get; set; }

            public static BookMove Parse(JToken json)
            {
                var toReturn = new BookMove();
                toReturn.White = json["white"].Value<int>();
                toReturn.Black = json["black"].Value<int>();
                toReturn.Draws = json["draws"].Value<int>();
                toReturn.Uci = json["uci"].Value<string>();
                toReturn.San = json["san"].Value<string>();
                return toReturn;
            }
        }
    }
}