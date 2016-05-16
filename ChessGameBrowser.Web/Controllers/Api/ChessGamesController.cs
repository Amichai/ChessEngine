using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using ChessGameBrowser.Web.Areas.HelpPage.ModelDescriptions;
using ChessGameBrowser.Web.Data;
using ChessGameBrowser.Web.Models;
using Newtonsoft.Json.Linq;

namespace ChessGameBrowser.Web.Controllers.Api
{
    public class ChessGamesController : ApiController
    {
        public JArray Get()
        {
            var games = DynamoDBConnection.Instance.GetAllGames();
            var arr = new JArray();
            foreach (var game in games)
            {
                arr.Add(game.ToJson());
            }

            return arr;
        }

        public JObject Get(string id)
        {
            return DynamoDBConnection.Instance.GetGame(id).ToJson();
        }

        public void Post()
        {
            var path = HostingEnvironment.MapPath("~/App_Data/data.json");
            var text = File.ReadAllText(path);
            var jo = JObject.Parse(text);
            var packages = (jo["packages"] as JArray);
            foreach (var package in packages)
            {
                List<GamesModel> games;

                var packageModel = PackageModel.FromCompleteJson(package as JObject, out games);

                List<string> ids =  new List<string>();
                foreach (var game in games)
                {
                    var id = DynamoDBConnection.Instance.AddGame(game);
                    ids.Add(id);
                }
                packageModel.Games = ids;
                DynamoDBConnection.Instance.AddPackage(packageModel);

            }
        }


        private void post(string val)
        {
            var model = GamesModel.FromJson(JObject.Parse(val));


            DynamoDBConnection.Instance.AddGame(model);
        }
    }
}
