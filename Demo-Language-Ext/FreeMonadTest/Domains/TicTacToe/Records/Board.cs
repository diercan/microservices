using FreeMonadTest.Domains.TicTacToe.Records;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FreeMonadTest.Domains.TicTacToe.Records.Board;
using static LanguageExt.Prelude;

namespace FreeMonadTest.Domains.TicTacToe.Records
{
    public partial class Board : Record<Board>
    {
        public readonly Lst<Lst<Option<Shapes>>> Cells;
        public readonly Shapes NextMoveShape;
        public readonly GameStatus Status;
        public readonly Option<Shapes> Winner;

        public Dimension Size { get => new Dimension(Cells.Count); }
        public string WinnerShape
        {
            get
            {
                return Winner.Match(
                        Some: s => (s == Shapes.X ? "X" : "O"),
                        None: () => string.Empty
                    );
            }
        }

        internal Board(Dimension n)
        {
            Cells = CreateEmptyBoard(n);
            NextMoveShape = Shapes.X;
            Status = GameStatus.InProgress;
            Winner = Option<Shapes>.None;
        }

        public Board(Lst<Lst<Option<Shapes>>> cells)
        {
            if (!cells.ForAll(row => row.Length() == cells.Length()) || cells.Length() > Dimension.MaxBoardDimension) throw new ArgumentOutOfRangeException("Invalid board");

            Cells = cells;
            NextMoveShape = CalculateNextMoveShape();
            Status = CalculateGameStatus();
            if (IsXWinner())
            {
                Status = GameStatus.Over;
                Winner = Some(Shapes.X);
            }

            if (IsOWinner())
            {
                Status = GameStatus.Over;
                Winner = Some(Shapes.O);
            }
        }

        public Validation<string, Point> CreateBoardPoint(Coordinate x, Coordinate y)
        {
            Validation<string, Point> result;
            if(x.Value<=Size.Value && y.Value <= Size.Value)
            {
                result = Success<string, Point>(new Point(x, y));
            }
            else
            {
                result = Fail<string, Point>("Point is outside board boundaries.");
            }

            return result;
        }

        public Option<Shapes> GetCell(Point P)
        {
            return Cells[P.X.Value-1][P.Y.Value-1];
        }

        #region moves logic
        private Shapes CalculateNextMoveShape()
        {
            var xMoveCount = (from row in Cells
                              from cell in row
                              from value in cell
                              where value == Shapes.X
                              select value)
                             .Count();
            var oMoveCount = (from row in Cells
                              from cell in row
                              from value in cell
                              where value == Shapes.O
                              select value)
                             .Count();
            return xMoveCount <= oMoveCount?Shapes.X: Shapes.O;
        }

        private GameStatus CalculateGameStatus()
        {
            return (from row in Cells
                    from cell in row
                    where cell.IsNone
                    select cell).Any()?GameStatus.InProgress:GameStatus.Over;
        }
        #endregion moves logic

        #region winner detection
        private bool IsXWinner()
        {
            return CheckLineWiner(Shapes.X) 
                    || CheckColumnWinner(Shapes.X) 
                    || CheckDiagonalWinner(Shapes.X);
        }

        private bool IsOWinner()
        {
            return CheckLineWiner(Shapes.O) 
                || CheckColumnWinner(Shapes.O) 
                || CheckDiagonalWinner(Shapes.O);
        }

        private bool CheckDiagonalWinner(Shapes shape)
        {
            var firstDiagonal = Range(1, Size.Value).Select(idx => Cells[idx-1][idx-1]);
            var secondDiagonal = Range(1, Size.Value).Select(idx => Cells[idx - 1][Size.Value - idx]);

            return firstDiagonal.ForAll(cell => cell.Match(Some: s => s == shape, None: () => false))
                || secondDiagonal.ForAll(cell => cell.Match(Some: s => s == shape, None: () => false));
        }

        private bool CheckColumnWinner(Shapes shape)
        {
            var pivot = Range(1, Size.Value).Select(colNum => from row in Cells
                                                             select row[colNum - 1]);
            return (from column in pivot
                    select column.ForAll(cell => cell.Match(Some: s => s == shape, None: () => false)))
                   .Any(s => s);
        }

        private bool CheckLineWiner(Shapes shape)
        {
            return (from row in Cells
                   select row.ForAll(cell => cell.Match(Some: s => s==shape, None: () => false)))
                   .Any(s=>s);
        }
        #endregion winner detection

        #region board generation
        private Lst<Lst<Option<Shapes>>> CreateEmptyBoard(Dimension n)
        {
            return Lst<Lst<Option<Shapes>>>
                            .Empty
                            .AddRange(
                                Range(1, n.Value).Select(r => CreateEmptyLine(n))
                            );
        }

        private static Lst<Option<Shapes>> CreateEmptyLine(Dimension n)
        {
            return Lst<Option<Shapes>>
                        .Empty
                        .AddRange(
                            Range(1, n.Value).Select(c => Option<Shapes>.None)
                        );
        }
        #endregion board generation

        #region toString
        public override string ToString()
        {
            var builder = Cells.Fold(
                    new StringBuilder().AppendLine("------------------"),
                    (s, x) => s.AppendLine(FoldRow(x))
                ).AppendLine("------------------");

            if (Status == GameStatus.Over)
            {
                Winner.Match(
                        Some: s => builder.AppendLine($"GAME OVER! {WinnerShape} wins!"),
                        None: () => builder.AppendLine("GAME OVER! Nobody wins!")
                    );
            }

            return builder.ToString();
        }

        private string FoldRow(Lst<Option<Shapes>> row)
        {
            return row.Fold(new StringBuilder().Append("|"), 
                    (s, x) => s.Append(x.Match(
                            Some: shape => $"{shape.ToString()}|",
                            None: () => " |"))).ToString();
        }
        #endregion toString
    }
}
