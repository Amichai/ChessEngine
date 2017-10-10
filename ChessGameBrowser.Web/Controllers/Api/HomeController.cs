using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using ChessGameBrowser.Web.Models;
using ChessKit.ChessLogic;
using ilf.pgn;
using Newtonsoft.Json.Linq;

namespace ChessGameBrowser.Web.Controllers.Api
{
    public class HomeController : ApiController
    {
        private Position ExecuteMove(Position position, string move)
        {
            var m = Move.Parse(move);
            var m2 = position.ValidateLegalMove(m);

            if (m2 == null)
            {
                return null;
            }

            position = m2.ToPosition();

            return position;
        }

        [HttpGet]
        public EvaluationResult Evaluation(string boardState)
        {
            string bestLine;
            var eval = ComputerEvaluation.Eval(boardState, out bestLine);
            return new EvaluationResult(eval, bestLine);
        }

        // POST: api/HomeApi
        public async Task<MoveResponse> PostMove()
        {
            var provider = new LiChessMoveProvider();
            var r = await Request.Content.ReadAsStringAsync();

            var jo = JObject.Parse(r);
            var position = Fen.Parse(jo["fen"].Value<string>());

            var move = jo["move"].Value<string>();

            var newPosition = ExecuteMove(position, move);

            if (newPosition  == null)
            {
                return MoveResponse.Invalid();
            }

            var targetMoves = provider.Get(newPosition);

            if (!targetMoves.Moves.Any() || !jo["autoMove"].Value<bool>())
            {
                return new MoveResponse(targetMoves, Fen.Print(newPosition), "");
            }

            var toExecute = targetMoves.Moves[random.Next(targetMoves.Moves.Take(5).Count())];

            newPosition = ExecuteMove(newPosition, toExecute.Move);

            Thread.Sleep(600);

            return new MoveResponse(provider.Get(newPosition), Fen.Print(newPosition), toExecute.Move);
        }

        private static Random random = new Random((int)DateTime.Now.TimeOfDay.Ticks);

        public async Task<List<InaccurateMoveModel>> Analyze()
        {
            var r = await Request.Content.ReadAsStringAsync();
            var provider = new LiChessMoveProvider();

            var reader = new PgnReader();
            var db = reader.ReadFromString(r);

            var currentPosition = Fen.StartingPosition;

            var inaccuracies = new List<InaccurateMoveModel>();

            foreach (var game in db.Games)
            {
                TargetMove nextMove = null;
                TargetMoves targetMoves = null;
                var moves = game.MoveText.GetMoves();
                foreach (var move in moves)
                {
                    var m = San.Parse(move.ToString(), currentPosition);
                    var legal = currentPosition.ValidateLegalMove(m.Move);

                    if (nextMove != null && San.ToString(legal) != nextMove.San)
                    {
                        inaccuracies.Add(new InaccurateMoveModel(currentPosition, move.ToString(), targetMoves));
                    }

                    var a = legal.ToPosition();

                    Thread.Sleep(300);
                    targetMoves = provider.Get(a);

                    nextMove = targetMoves.Moves.FirstOrDefault();
                    if (nextMove == null)
                    {
                        break;
                    }

                    currentPosition = a;
                }
            }

            return inaccuracies;
        }
    }
}
