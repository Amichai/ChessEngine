using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using ChessGameBrowser.Web.Models;
using log4net;
using Newtonsoft.Json.Linq;

namespace ChessGameBrowser.Web.Data
{
    public sealed class DynamoDBConnection
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IAmazonDynamoDB client;

        private DynamoDBConnection(RegionEndpoint endpoint = null)
        {
            if (endpoint == null)
            {
                endpoint = RegionEndpoint.USEast1;
            }
            client = new AmazonDynamoDBClient(endpoint);
        }

        public static DynamoDBConnection Instance = new DynamoDBConnection();

        private List<Dictionary<string, string>> get(
            string table,
            string keyName,
            string keyValue)
        {
            string prefixedKeyName = ":v_" + keyName;
            var av = new AttributeValue { S = keyValue };
            var response = this.client.Query(new QueryRequest(table)
            {
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>() {
                    { prefixedKeyName, av}
                },
                KeyConditionExpression = string.Format("{0} = {1}", keyName, prefixedKeyName),
            });
            if (response.Items.Count == 0)
            {
                return new List<Dictionary<string, string>>();
            }
            log.InfoFormat("Queried: {0}, {1}, {2}", table, keyName, keyValue);
            return response.Items.Select(i => i.ToDictionary(j => j.Key, j => j.Value.S)).ToList();
        }

        private const string GAMES_TABLE = @"chess_games";
        private const string PACKAGES_TABLE = @"chess_packages";

        public enum ValueType { S, N };
        public struct TableAttribute
        {
            public TableAttribute(string name, string value, ValueType type = ValueType.S)
                : this()
            {
                this.AttributeName = name;
                this.AttributeValue = value;
                this.AttributeType = type;
            }
            public string AttributeValue { get; private set; }
            public string AttributeName { get; private set; }
            public ValueType AttributeType { get; set; }
            public AttributeValue ToAttributeValue()
            {
                switch (this.AttributeType)
                {
                    case ValueType.S:
                        return new AttributeValue { S = this.AttributeValue };
                    case ValueType.N:
                        return new AttributeValue { N = this.AttributeValue };
                    default:
                        throw new Exception("Unknown type");
                }
            }
        }

        private void add(string table, string keyName, string key, params TableAttribute[] values)
        {
            Dictionary<string, AttributeValue> attributes = new Dictionary<string, AttributeValue>();
            attributes[keyName] = new AttributeValue { S = key };
            foreach (var v in values)
            {
                attributes[v.AttributeName] = new AttributeValue { S = v.AttributeValue };
            }
            var response = client.BatchWriteItem(new BatchWriteItemRequest()
            {
                RequestItems = new Dictionary<string, List<WriteRequest>>() {
                    {
                      table,
                      new List<WriteRequest>() {
                           new WriteRequest(
                               new PutRequest(
                                   attributes
                                   )
                                )
                            }
                        }
                    },
            }
            );
            Debug.Assert(response.HttpStatusCode == System.Net.HttpStatusCode.OK);
            Debug.Assert(response.UnprocessedItems.Count == 0);
            log.Info(string.Format("Wrote Table: {0}, key: {1}, values: {2}", table, key, string.Join(",", values.Select(i => i.AttributeValue))));
        }


        private void delete(string table, string keyName, string keyValue)
        {
            this.client.DeleteItem(table,
                new Dictionary<string, AttributeValue>() {
                    {
                        keyName, new AttributeValue { S = keyValue }
                    }
                }
            );
            log.InfoFormat("Delete: {0}, {1}, {2}", table, keyValue, keyName);
        }

        private List<Dictionary<string, string>> scan(string table, int limit)
        {
            var response = this.client.Scan(new ScanRequest(table)
            {
                Limit = limit,
            });
            var toReturn = response.Items.Select(i => i.ToDictionary(j => j.Key, j => j.Value.S)).ToList();
            log.InfoFormat("Scan got {0} from: {1}", toReturn.Count, table);
            return toReturn;
        }

        public GamesModel GetGame(string gameId)
        {
            var indexName = "GameId";
            var propertyName = "GameId";
            var matches = this.get(GAMES_TABLE, propertyName, gameId);
            log.InfoFormat("Queried {0}, {1}, {2}, {3}", GAMES_TABLE, indexName, propertyName, gameId);
            var match = matches.SingleOrDefault();
            return GamesModel.FromDictionary(match);
        }

        public List<GamesModel> GetAllGames()
        {
            return this.scan(GAMES_TABLE, 1000).Select(i => {
                var info = JObject.Parse(i["GameInfo"]);
                return GamesModel.FromJson(info);
            }).ToList();
        }

        public List<PackageModel> GetAllPackages()
        {
            return this.scan(PACKAGES_TABLE, 1000).Select(i => {
                var info = JObject.Parse(i["PackageInfo"]);
                return PackageModel.FromJson(info);
            }).ToList();
        }

        public string AddGame(GamesModel game)
        {
            var gameId = Guid.NewGuid().ToString();
            this.add(GAMES_TABLE, "GameId", gameId, game.GetTableAttributes());
            return gameId;
        }

        public void AddPackage(PackageModel package)
        {
            this.add(PACKAGES_TABLE, "PackageId", Guid.NewGuid().ToString(), package.GetPackageAttributes());
        }
    }
}