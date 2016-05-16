using System;
using System.Collections.Generic;
using System.Linq;
using ChessGameBrowser.Web.Data;
using Newtonsoft.Json.Linq;

namespace ChessGameBrowser.Web.Models
{
    public sealed class GamesModel
    {
        public string White { get; private set; }
        public string Black { get; private set; }
        public int Year { get; private set; }
        public string Moves { get; private set; }
        public string[] Annotations { get; private set; }

        public JArray AnnotationJArray
        {
            get
            {
                return new JArray(Annotations);
            }
        }

        public static GamesModel FromDictionary(Dictionary<string, string> kvs)
        {
            var info = JObject.Parse(kvs["GameInfo"]);

            return FromJson(info);
        }

        public static GamesModel FromJson(JObject json)
        {
            var model = new GamesModel();
            model.White = json["white"].ToString();
            model.Black = json["black"].ToString();
            model.Year = json["year"].Value<int>();
            model.Moves = json["moves"].ToString();
            model.Annotations = (json["annotations"] as JArray).Select(i => i.ToString()).ToArray();

            return model;
        }

        public JObject ToJson()
        {
            var jo = new JObject();
            jo["white"] = White;
            jo["black"] = Black;
            jo["year"] = Year;
            jo["moves"] = Moves;
            jo["annotations"] = new JArray(Annotations);
            return jo;
        }

        public DynamoDBConnection.TableAttribute[] GetTableAttributes()
        {
            return new[]
            {
                new DynamoDBConnection.TableAttribute("GameInfo", ToJson().ToString())
            };
        }
    }
}