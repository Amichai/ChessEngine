using System.Runtime.Serialization;

namespace ChessGameBrowser.Web.Models
{
    [DataContract]
    public sealed class EvaluationResult
    {
        public EvaluationResult(string eval, string bestLine)
        {
            this.Eval = eval;
            this.BestLine = bestLine;
        }

        [DataMember]
        public string Eval { get; set; }

        [DataMember]
        public string BestLine { get; set; }
    }
}