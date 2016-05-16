using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ChessGameBrowser.Web.Data;
using Newtonsoft.Json.Linq;

namespace ChessGameBrowser.Web.Controllers.Api
{
    public class PackageController : ApiController
    {
        public JArray Get()
        {
            var allPackages = DynamoDBConnection.Instance.GetAllPackages();
            var arr = new JArray();
            foreach (var package in allPackages)
            {
                arr.Add(package.ToJson());
            }

            return arr;
        }
    }
}
