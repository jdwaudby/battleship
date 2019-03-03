using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battleship.Library.Models
{
    public class Grid
    {
        private const char CellLeftTop = '┌';
        private const char CellRightTop = '┐';
        private const char CellLeftBottom = '└';
        private const char CellRightBottom = '┘';
        private const char CellHorizontalJointTop = '┬';
        private const char CellHorizontalJointBottom = '┴';
        private const char CellVerticalJointLeft = '├';
        private const char CellTJoint = '┼';
        private const char CellVerticalJointRight = '┤';
        private const char CellHorizontalLine = '─';
        private const char CellVerticalLine = '│';

        public Square[,] Squares { get; protected set; }

        public Grid(int width, int height)
        {
            Squares = new Square[width, height];
        }

        public override string ToString()
        {
            int maxX = Squares.GetLength(0);
            int maxY = Squares.GetLength(1);

            var rows = new List<string>();

            string row0 = "  ";
            string row1 = $" {CellLeftTop}";
            string row2 = $"{CellVerticalLine}";
            string row3 = $" {CellVerticalJointLeft}";
            string row4 = $" {CellLeftBottom}";

            for (int x = 0; x < maxX; x++)
            {
                row0 += $"{x.ToString().Last()} ";
                row1 += $"{CellHorizontalLine}{CellHorizontalJointTop}";
                row2 += $" {CellVerticalLine}";
                row3 += $"{CellHorizontalLine}{CellTJoint}";
                row4 += $"{CellHorizontalLine}{CellHorizontalJointBottom}";
            }

            for (int y = 0; y < maxY; y++)
            {
                rows.Add(y.ToString().Last() + row2);
                rows.Add(row3);
            }

            rows.RemoveAt(rows.Count - 1);

            var sb = new StringBuilder();

            sb.AppendLine(row0);
            sb.AppendLine(row1.TrimEnd(CellHorizontalJointTop) + CellRightTop);

            foreach (string row in rows)
            {
                if (row.EndsWith($"{CellTJoint}"))
                {
                    sb.AppendLine(row.TrimEnd(CellTJoint) + CellVerticalJointRight);
                }
                else
                {
                    sb.AppendLine(row);
                }
            }

            sb.AppendLine(row4.TrimEnd(CellHorizontalJointBottom) + CellRightBottom);

            return sb.ToString();
        }
    }
}
