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
        public IReadOnlyList<Square> Squares { get; }

        public Grid(int width, int height)
        {
            var squares = new List<Square>();

            for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
            {
                string y = IntegerToLetterSequence(i);
                int x = j + 1;

                squares.Add(new Square(y, x));
            }

            Squares = squares;
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

            string row0 = $" {LeftTop}";
            string row2 = $" {VerticalJointLeft}";
            string row3 = $" {LeftBottom}";
            string row4 = "  ";

            foreach (string x in Squares.Select(square => square.X).Distinct())
            {
                string horizontalLine = new string(HorizontalLine, x.Length);
                row0 += $"{horizontalLine}{HorizontalJointTop}";
                row2 += $"{horizontalLine}{CentreJoint}";
                row3 += $"{horizontalLine}{HorizontalJointBottom}";
                row4 += $"{x} ";
            }

            var rows = new List<string>();

            var squaresGroupedByY = Squares.GroupBy(square => square.Y);
            foreach (var squareGroup in squaresGroupedByY)
            {
                // Todo: Remove .Last()
                string row = $"{squareGroup.Key.Last()}{VerticalLine}";

                foreach (Square square in squareGroup)
                {
                    string padding = new string(' ', square.X.Length - 1);
                    var squareStatus = square.Status;
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

        private static string IntegerToLetterSequence(int value)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            
            string result = "";

            if (value >= letters.Length)
                result += letters[value / letters.Length - 1];

            result += letters[value % letters.Length];

            return result;
        }
    }
}
