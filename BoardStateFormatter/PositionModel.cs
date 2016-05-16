using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using ChessKit.ChessLogic;

namespace BoardStateFormatter
{
    internal sealed class PositionModel
    {
        private readonly string unformatted;

        private static readonly Dictionary<char, Piece> PieceParser = new Dictionary<char, Piece>
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

        private Piece?[] pieces = new Piece?[64];

        public Piece?[] Pieces
        {
            get { return pieces; }
        }

        public bool IsWhitesTurnToMove
        {
            get; private set;
        }

        public PositionModel(Position position)
            : this(Fen.Print(position).Split(' ').First())
        {
        }

        public PositionModel(string unformatted)
        {
            this.unformatted = unformatted;
            var rows = unformatted.Split('/');
            var pieceIndex = 0;
            foreach (var row in rows)
            {
                foreach (var c in row)
                {
                    if (char.IsDigit(c))
                    {
                        for (var i = 0; i < int.Parse(c.ToString()); i++)
                        {
                            Pieces[pieceIndex++] = null;
                        }

                        continue;
                    }

                    Pieces[pieceIndex++] = PieceParser[c];
                }
            }
        }

        private string ToString(Piece piece)
        {
            return PieceParser.Single(i => i.Value == piece).Key.ToString();
        }

        public override string ToString()
        {
            var toReturn = new StringBuilder();

            var idx = 1;
            foreach (var piece in Pieces)
            {
                if (piece.HasValue)
                {
                    toReturn.Append("|" + ToString(piece.Value));
                }
                else
                {
                    toReturn.Append("| ");
                }

                if (idx++%8==0)
                {
                    toReturn.Append("|\n");
                }
            }

            return toReturn.ToString();
        }

        public List<CellPieceModel> PieceCells()
        {
            List <CellPieceModel> toReturn = new List<CellPieceModel>();
            for (int i = 0; i < Pieces.Length; i++)
            {
                var p = Pieces[i];

                if (!p.HasValue)
                {
                    continue;
                }

                toReturn.Add(new CellPieceModel(i, p));
            }

            return toReturn;
        }

        public MoveModel Diff(PositionModel other)
        {
            var toReturn = new MoveModel();

            for (int i = 0; i < 64; i++)
            {
                var a = Pieces[i];
                var b = other.Pieces[i];
                if (a == b)
                {
                    continue;
                }

                if (b != null)
                {
                    toReturn.Added.Add(new CellPieceModel(i, b));
                }

                if (a != null)
                {
                    toReturn.Removed.Add(new CellPieceModel(i, a));
                }
            }

            return toReturn;
        }

        internal PositionModel Move(MoveModel move)
        {
            foreach (var removed in move.Removed)
            {
                Pieces[removed.Idx] = null;
            }

            foreach (var added in move.Added)
            {
                Pieces[added.Idx] = added.Piece;
            }

            IsWhitesTurnToMove = !IsWhitesTurnToMove;

            return this;
        }

        public Piece? Get(CellModel cell)
        {
            return Pieces[cell.Idx];
        }

        private PositionModel()
        {

        }

        public PositionModel Clone()
        {
            return new PositionModel()
            {
                pieces = Pieces.ToArray()
            };
        }
    }

    public enum Piece
    {
        WhitePawn,
        WhiteKnight,
        WhiteBishop,
        WhiteRook,
        WhiteKing,
        WhiteQueen,
        BlackPawn,
        BlackKnight,
        BlackBishop,
        BlackRook,
        BlackKing,
        BlackQueen
    }
}