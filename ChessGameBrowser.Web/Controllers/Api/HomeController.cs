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
using ChessKit.ChessLogic.Algorithms;
using Newtonsoft.Json.Linq;

namespace ChessGameBrowser.Web.Controllers.Api
{
    public class HomeController : ApiController
    {
        private Position ExecuteMove(Position position, string move)
        {
            var m = Move.Parse(move);
            var m2 = position.ValidateLegal2(m);

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

            var position = jo["fen"].Value<string>().ParseFen();

            var move = jo["move"].Value<string>();

            var newPosition = ExecuteMove(position, move);

            if (newPosition  == null)
            {
                return MoveResponse.Invalid();
            }

            var targetMoves = provider.Get(newPosition);

            if (!targetMoves.Moves.Any() || !jo["autoMove"].Value<bool>())
            {
                return new MoveResponse(targetMoves, newPosition.PrintFen(), "");
            }

            var toExecute = targetMoves.Moves[random.Next(targetMoves.Moves.Take(5).Count())];

            newPosition = ExecuteMove(newPosition, toExecute.Move);

            Thread.Sleep(600);

            return new MoveResponse(provider.Get(newPosition), newPosition.PrintFen(), toExecute.Move);
        }

        private static Random random = new Random((int)DateTime.Now.TimeOfDay.Ticks);
    }
}
