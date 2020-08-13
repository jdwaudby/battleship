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
            string row1 = $" {LeftTop}";
            string row3 = $" {VerticalJointLeft}";
            string row4 = $" {LeftBottom}";

            for (int x = 0; x < maxX; x++)
            {
                row0 += $"{x.ToString().Last()} ";
                row1 += $"{HorizontalLine}{HorizontalJointTop}";
                row3 += $"{HorizontalLine}{CentreJoint}";
                row4 += $"{HorizontalLine}{HorizontalJointBottom}";
            }

            var rows = new List<string>();
            
            for (int y = 0; y < maxY; y++)
            {
                var row = $"{y.ToString().Last()}{VerticalLine}";

                for (int x = 0; x < maxX; x++)
                {
                    row += $"{gridValues[Squares[x,y].Status]}{VerticalLine}";
                }

                rows.Add(row);
            }

            var sb = new StringBuilder();

            sb.AppendLine(row0);
            sb.AppendLine(row1.TrimEnd(HorizontalJointTop) + RightTop);

            foreach (string row in rows)
            {
                sb.AppendLine(row);

                if (row != rows.Last())
                    sb.AppendLine(row3.TrimEnd(CentreJoint) + VerticalJointRight);
            }

            sb.AppendLine(row4.TrimEnd(HorizontalJointBottom) + RightBottom);

            return sb.ToString();
        }
    }
}
