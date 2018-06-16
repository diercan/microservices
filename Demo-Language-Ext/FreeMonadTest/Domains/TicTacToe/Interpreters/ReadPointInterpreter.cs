using FreeMonadTest.Domains.TicTacToe.Operations;
using FreeMonadTest.Interpreters;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FreeMonadTest.Domains.TicTacToe.Records.Board;
using static LanguageExt.Prelude;

namespace FreeMonadTest.Domains.TicTacToe.Interpreters
{
    public class ReadPointInterpreter : OpInterpreter<ReadPoint, Validation<string, Point>>
    {
        public override Validation<string, Point> Mock(ReadPoint Op)
        {
            return Op.Board.CreateBoardPoint(new Coordinate(1), new Coordinate(1));
        }

        public override Validation<string, Point> Work(ReadPoint Op)
        {
            var point = from x in ReadValidCoorindate("X", Op)
                        from y in ReadValidCoorindate("Y", Op)
                        select Op.Board.CreateBoardPoint(x, y);

            return point.Match(
                    Some: p => p,
                    None: () => Fail<string, Point>("Could not read point")
                );
        }

        private Option<Coordinate> ReadValidCoorindate(string label, ReadPoint op)
        {
            var x = Option<Coordinate>.None;
            int tries = 0;
            do
            {
                Console.Write($"{label}:");
                var readX = parseInt(Console.ReadLine());
                x = readX.Match(
                    Some: xVal => (xVal >= 1 && xVal <= op.Board.Size.Value)?Some(new Coordinate(xVal)):x,
                    None: () => x);
                tries++;
            } while (x.IsNone && tries < 4);
            return x;
        }
    }
}
