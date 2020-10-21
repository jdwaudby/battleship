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
        private const char LeftTop = '┌';
        private const char RightTop = '┐';
        private const char LeftBottom = '└';
        private const char RightBottom = '┘';
        private const char HorizontalJointTop = '┬';
        private const char HorizontalJointBottom = '┴';
        private const char VerticalJointLeft = '├';
        private const char CentreJoint = '┼';
        private const char VerticalJointRight = '┤';
        private const char HorizontalLine = '─';
        private const char VerticalLine = '│';

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

            string row0 = $" {LeftTop}";
            string row2 = $" {VerticalJointLeft}";
            string row3 = $" {LeftBottom}";
            string row4 = "  ";

            for (int x = 0; x < maxX; x++)
            {
                string coord = (x + 1).ToString();
                string horizontalLine = new string(HorizontalLine, coord.Length);
                row0 += $"{horizontalLine}{HorizontalJointTop}";
                row2 += $"{horizontalLine}{CentreJoint}";
                row3 += $"{horizontalLine}{HorizontalJointBottom}";
                row4 += $"{coord} ";
            }

            var rows = new List<string>();
            
            for (int y = 0; y < maxY; y++)
            {
                string row = $"{y.ToString().Last()}{VerticalLine}";

                for (int x = 0; x < maxX; x++)
                {
                    string padding = new string(' ', (x + 1).ToString().Length - 1);
                    var squareStatus = Squares[x, y].Status;
                    row += $"{(squareStatus.HasValue ? gridValues[squareStatus.Value] : " ")}{padding}{VerticalLine}";
                }

                rows.Add(row);
            }

            var sb = new StringBuilder();

            sb.AppendLine(row0.TrimEnd(HorizontalJointTop) + RightTop);

            foreach (string row in rows)
            {
                sb.AppendLine(row);

                if (row != rows.Last())
                    sb.AppendLine(row2.TrimEnd(CentreJoint) + VerticalJointRight);
            }

            sb.AppendLine(row3.TrimEnd(HorizontalJointBottom) + RightBottom);
            sb.AppendLine(row4);

            return sb.ToString();
        }
    }
}
