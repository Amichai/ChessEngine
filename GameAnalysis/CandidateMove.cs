using System;
using System.Collections.Generic;
using System.Linq;

namespace GameAnalysis
{
    internal sealed class CandidateMove
    {
        public string Move { get; }

        private double? eval;

        public double? Eval
        {
            get
            {
                if (Mate != 0)
                {
                    return null;
                }

                return eval;
            }
        }

        public int Mate { get; }

        public List<string> InspectionLine
        {
            get;
            private set;
        }

        public CandidateMove(string move, double? eval, int mate = 0)
        {
            Move = move;
            this.eval= eval;
            Mate = mate;
        }

        public static CandidateMove Parse(string inspectionLine, bool isBlack)
        {
            var parts = inspectionLine.Split(new[] {"pv "}, StringSplitOptions.None).Last().Split(' ');

            var m = parts.First().TrimEnd();
            var e = int.Parse(inspectionLine.Split(new[] { "cp " }, StringSplitOptions.None).Last().Split(' ').First().TrimEnd()) / 100d;

            if (isBlack)
            {
                e *= -1;
            }

            var toReturn = new CandidateMove(m, e);
            
            toReturn.InspectionLine = parts.Skip(1).Select(i => i.Trim()).ToList();

            return toReturn;
        }
    }
}