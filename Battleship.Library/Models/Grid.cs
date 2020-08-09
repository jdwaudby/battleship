using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Battleship.Library.Enums;

namespace Battleship.Library.Models
{
    public class Grid : IFormattable
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

        public Square[,] Squares { get; }

        public Grid(int width, int height)
        {
            Squares = new Square[width, height];
        }

        public override string ToString()
        {
            return ToString("G", CultureInfo.CurrentCulture);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            bool positioning = format.Equals("Positioning");
            bool targeting = format.Equals("Targeting");

            var gridValues = new Dictionary<SquareStatus, string>
            {
                {SquareStatus.Empty, " "},
                {SquareStatus.AircraftCarrier, positioning ? "A" : " "},
                {SquareStatus.Battleship, positioning ? "B" : " "},
                {SquareStatus.Cruiser, positioning ? "C" : " "},
                {SquareStatus.Submarine, positioning ? "S" : " "},
                {SquareStatus.Destroyer, positioning ? "D" : " "},
                {SquareStatus.Hit, targeting ? "H" : " "},
                {SquareStatus.Miss, targeting ? "M" : " "},
                {SquareStatus.Ship, positioning ? "S" : " "}
            };

            int maxX = Squares.GetLength(0);
            int maxY = Squares.GetLength(1);

            string row0 = "  ";
            string row1 = $" {CellLeftTop}";
            string row3 = $" {CellVerticalJointLeft}";
            string row4 = $" {CellLeftBottom}";

            for (int x = 0; x < maxX; x++)
            {
                row0 += $"{x.ToString().Last()} ";
                row1 += $"{CellHorizontalLine}{CellHorizontalJointTop}";
                row3 += $"{CellHorizontalLine}{CellTJoint}";
                row4 += $"{CellHorizontalLine}{CellHorizontalJointBottom}";
            }

            var rows = new List<string>();
            
            for (int y = 0; y < maxY; y++)
            {
                var row = $"{y.ToString().Last()}{CellVerticalLine}";

                for (int x = 0; x < maxX; x++)
                {
                    row += $"{gridValues[Squares[x,y].Status]}{CellVerticalLine}";
                }

                rows.Add(row);
            }

            var sb = new StringBuilder();

            sb.AppendLine(row0);
            sb.AppendLine(row1.TrimEnd(CellHorizontalJointTop) + CellRightTop);

            foreach (string row in rows)
            {
                sb.AppendLine(row);

                if (row != rows.Last())
                    sb.AppendLine(row3.TrimEnd(CellTJoint) + CellVerticalJointRight);
            }

            sb.AppendLine(row4.TrimEnd(CellHorizontalJointBottom) + CellRightBottom);

            return sb.ToString();
        }
    }
}
