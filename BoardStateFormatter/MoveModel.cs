using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

namespace BoardStateFormatter
{
    internal sealed class MoveModel
    {
        public List<CellPieceModel> Removed = new List<CellPieceModel>();
        public List<CellPieceModel> Added = new List<CellPieceModel>();

        public override string ToString()
        {
            var a = string.Join(",", Removed);
            var b = string.Join(",", Added);

            return string.Format("{0}:{1}", a, b);
        }

        public static MoveModel Parse(string val)
        {
            var s = val.Split(':');
            var removed = s[0];
            var added = s[1];

            return new MoveModel()
            {
                Removed = parse(removed),
                Added = parse(added),
            };
        }

        private static CellPieceModel parseSingleDiff(string val)
        {
            string digitComponent = string.Empty;
            int lastDigit = 0;

            foreach (var @char in val)
            {
                if (char.IsDigit(@char))
                {
                    digitComponent += @char;
                }
                else if (digitComponent != string.Empty)
                {
                    lastDigit = int.Parse(digitComponent);
                    digitComponent = string.Empty;
                }
            }

            return new CellPieceModel(lastDigit, NotationToPiece[val.Last()]);

        }

        private static List<CellPieceModel> parse(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return new List<CellPieceModel>();
            }

            if (val.Contains(","))
            {
                List<CellPieceModel> toReturn = new List<CellPieceModel>();

                foreach (var part in val.Split(','))
                {
                    toReturn.Add(parseSingleDiff(part));
                }

                return toReturn;

            }

            return new List<CellPieceModel> {parseSingleDiff(val)};
        }

        public static readonly Dictionary<Piece, char> PieceToNotation = new Dictionary<Piece, char>
        {
            {Piece.BlackPawn,   'p'},
            {Piece.BlackKnight, 'n'},
            {Piece.BlackBishop, 'b'},
            {Piece.BlackRook,   'r'},
            {Piece.BlackKing,   'k'},
            {Piece.BlackQueen,  'q'},
            {Piece.WhitePawn,   'P'},
            {Piece.WhiteKnight, 'N'},
            {Piece.WhiteBishop, 'B'},
            {Piece.WhiteRook,   'R'},
            {Piece.WhiteKing,   'K'},
            {Piece.WhiteQueen,  'Q'}
        };

        private static readonly Dictionary<char, Piece> NotationToPiece = new Dictionary<char, Piece>
        {
            {'p', Piece.BlackPawn},
            {'n', Piece.BlackKnight},
            {'b', Piece.BlackBishop},
            {'r', Piece.BlackRook},
            {'k', Piece.BlackKing},
            {'q', Piece.BlackQueen},
            {'P', Piece.WhitePawn},
            {'N', Piece.WhiteKnight},
            {'B', Piece.WhiteBishop},
            {'R', Piece.WhiteRook},
            {'K', Piece.WhiteKing},
            {'Q', Piece.WhiteQueen}
        };
    }

    public class CellPieceModel
    {
        public readonly char Val;
        private readonly CellModel cell;

        public CellModel Cell
        {
            get { return cell; }
        }

        public int Idx
        {
            get { return cell.Idx; }
        }

        private CellPieceModel(Piece? piece)
        {
            if (!piece.HasValue) return;

            Piece = piece.Value;

            Val = MoveModel.PieceToNotation[Piece.Value];
        }

        public CellPieceModel(int x, int y, Piece? piece)
            : this(piece)
        {
            this.cell = new CellModel(x, y);
        }

        public CellPieceModel(int idx, Piece? piece)
            : this(piece)
        {
            this.cell = new CellModel(idx);
        }

        public CellPieceModel(CellModel cellModel, Piece? piece)
            : this(piece)
        {
            this.cell = cellModel;
        }

        public Piece? Piece { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}{1}", cell.Idx, Val);
        }

        public CellPieceModel Clone()
        {
            if (Piece.HasValue)
            {
                return new CellPieceModel(Idx, Piece.Value);
            }
            else
            {
                return new CellPieceModel(Idx, null);
            }
        }
    }

    public class CellModel
    {
        public readonly int Idx;

        public int X
        {
            get { return Idx % 8; }
        }

        public int Y
        {
            get { return Idx / 8; }
        }

        public CellModel(int idx)
        {
            this.Idx = idx;
        }

        public CellModel(int x, int y)
            : this(y * 8  + x)
        {
        }

        public CellModel Move(int dx, int dy)
        {
            var newX = X + dx;
            var newY = Y + dy;

            if (newX < 0 || newX >= 8 || newY < 0 || newY >= 8)
            {
                return null;
            }

            return new CellModel(X + dx, Y + dy);
        }

        public CellModel Clone()
        {
            return new CellModel(X, Y);
        }
    }
}