using FreeMonadTest.Domains.TicTacToe.Records;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FreeMonadTest.Domains.TicTacToe.Records.Board;
using static LanguageExt.Prelude;

namespace FreeMonadTest.Domains.TicTacToe.Operations
{
    public static class MoveOperation
    {
        public static Validation<string, Board> Move(this Board board, Validation<string, Point> point)
        {
            return from p in point
                   from validatedBoard in CheckValidGameStatus(board)
                   from validatedeCell in CheckCellStatus(board.GetCell(p))
                   select UpdateBoard(validatedBoard, p);
        }

        private static Board UpdateBoard(Board b, Point p)
        {
            var newRow = Lst<Option<Shapes>>.Empty;
            newRow = newRow.AddRange(b.Cells[p.X.Value - 1].Map((idx, cell) => idx == p.Y.Value - 1 ? Some(b.NextMoveShape) : cell));
            var newCells = Lst<Lst<Option<Shapes>>>.Empty;
            newCells = newCells.AddRange(b.Cells.Map((idx, row) => idx == p.X.Value - 1 ? newRow : row));
            return new Board(newCells);
        }

        private static Validation<string, Board> CheckValidGameStatus(Board b)
        {
            Validation<string, Board> result;
            if (b.Status == Board.GameStatus.InProgress)
            {
                result = Success<string, Board>(b);
            }
            else
            {
                result = Fail<string, Board>("Move is not possible when game is over");
            }
            return result;
        }

        private static Validation<string, Option<Shapes>> CheckCellStatus(Option<Shapes> cell)
        {
            Validation<string, Option<Shapes>> result;
            if (cell.IsNone)
            {
                result = Success<string, Option<Shapes>>(cell);
            }
            else
            {
                result = Fail<string, Option<Shapes>>("Move is not possible when cell is taken");
            }
            return result;
        }
    }
}
