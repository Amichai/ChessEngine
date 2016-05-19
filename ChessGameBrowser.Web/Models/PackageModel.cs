using System.Collections.Generic;
using System.Linq;
using ChessGameBrowser.Web.Data;
using Newtonsoft.Json.Linq;

namespace ChessGameBrowser.Web.Models
{
    public sealed class PackageModel
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public int Downloads { get; set; }
        public string Price { get; set; }
        public string Id { get; set; }
        public List<string> Games { get; set; }

        public static PackageModel FromJson(JObject json)
        {
            var toReturn = new PackageModel();
            toReturn.Name = json["name"].ToString();
            toReturn.Author = json["author"].ToString();
            toReturn.Downloads = json["downloads"].Value<int>();
            toReturn.Price = json["price"].ToString();
            toReturn.Games = (json["games"] as JArray).Select(i => i.ToString()).ToList();

            return toReturn;
        }

        public static PackageModel FromCompleteJson(JObject json, out List<GamesModel> games)
        {
            games = new List<GamesModel>();
            var toReturn = new PackageModel();
            toReturn.Name = json["name"].ToString();
            toReturn.Author = json["author"].ToString();
            toReturn.Downloads = json["downloads"].Value<int>();
            toReturn.Price = json["price"].ToString();
            var g = (json["games"] as JArray);
            foreach (var game in g)
            {
                games.Add(GamesModel.FromJson(game as JObject));
            }

            return toReturn;
        }

        public JObject ToJson()
        {
            var toReturn = new JObject();
            if (Id != null)
            {
                toReturn["id"] = Id;
            }

            toReturn["name"] = Name;
            toReturn["author"] = Author;
            toReturn["downloads"] = Downloads;
            toReturn["price"] = Price;
            toReturn["games"] = new JArray(Games);

            return toReturn;
        }

        public DynamoDBConnection.TableAttribute[] GetPackageAttributes()
        {
            return new[]
            {
                new DynamoDBConnection.TableAttribute("PackageInfo", ToJson().ToString())
            };
        }
    }
}