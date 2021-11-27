using System.Globalization;
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
                {SquareStatus.AircraftCarrier, positioning ? "a" : " "},
                {SquareStatus.Battleship, positioning ? "b" : " "},
                {SquareStatus.Cruiser, positioning ? "c" : " "},
                {SquareStatus.Submarine, positioning ? "s" : " "},
                {SquareStatus.Destroyer, positioning ? "d" : " "},
                {SquareStatus.Hit, targeting ? "h" : " "},
                {SquareStatus.Miss, targeting ? "m" : " "},
                {SquareStatus.Ship, positioning ? "s" : " "}
            };

            int yLength = Squares.Select(square => square.Y).Last().Length;
            string yPadding = new(' ', yLength);

            string row0 = $"{yPadding}{LeftTop}";
            string row2 = $"{yPadding}{VerticalJointLeft}";
            string row3 = $"{yPadding}{LeftBottom}";
            string row4 = $"{yPadding} ";

            foreach (string x in Squares.Select(square => square.X).Distinct())
            {
                string horizontalLine = new(HorizontalLine, x.Length);
                row0 += $"{horizontalLine}{HorizontalJointTop}";
                row2 += $"{horizontalLine}{CentreJoint}";
                row3 += $"{horizontalLine}{HorizontalJointBottom}";
                row4 += $"{x} ";
            }

            var rows = new List<string>();

            var squaresGroupedByY = Squares.GroupBy(square => square.Y);
            foreach (var squareGroup in squaresGroupedByY)
            {
                string row = $"{squareGroup.Key.PadLeft(yLength, ' ')}{VerticalLine}";

                foreach (Square square in squareGroup)
                {
                    string xPadding = new(' ', square.X.Length - 1);
                    var squareStatus = square.Status;
                    row += $"{(squareStatus.HasValue ? gridValues[squareStatus.Value] : " ")}{xPadding}{VerticalLine}";
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
