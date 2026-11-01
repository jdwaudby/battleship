using System.Globalization;
using System.Text;
using Battleship.Library.Enums;

namespace Battleship.Library.Models;

public class Grid : IFormattable
{
    private const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

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

    private readonly Dictionary<string, Square> _squareLookup;

    public IReadOnlyList<Square> Squares { get; }

    public Square? this[string coordinates] => _squareLookup.GetValueOrDefault(coordinates);

    public Grid(int width, int height)
    {
        if (width < 0 || height < 0)
        {
            Squares = [];
            _squareLookup = new Dictionary<string, Square>(StringComparer.OrdinalIgnoreCase);
            return;
        }

        List<Square> squares = [];
        _squareLookup = new Dictionary<string, Square>(width * height, StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < height; i++)
        for (int j = 0; j < width; j++)
        {
            string y = IntegerToLetterSequence(i);
            int x = j + 1;

            var square = new Square(y, x);
            squares.Add(square);
            _squareLookup[square.Coordinates] = square;
        }

        Squares = squares;
    }

    public override string ToString()
    {
        return ToString("G", CultureInfo.CurrentCulture);
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        format = format ?? throw new ArgumentNullException(nameof(format));

        bool positioning = format.Equals("Positioning");
        bool targeting = format.Equals("Targeting");

        int yLength = Squares[^1].Y.Length;
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

        List<string> rows = [];

        var squaresGroupedByY = Squares.GroupBy(square => square.Y);
        foreach (var squareGroup in squaresGroupedByY)
        {
            string row = $"{squareGroup.Key.PadLeft(yLength, ' ')}{VerticalLine}";

            foreach (Square square in squareGroup)
            {
                string xPadding = new(' ', square.X.Length - 1);
                var squareStatus = square.Status;
                string symbol = squareStatus.HasValue ? GetSquareSymbol(squareStatus.Value, positioning, targeting) : " ";
                row += $"{symbol}{xPadding}{VerticalLine}";
            }

            rows.Add(row);
        }

        var sb = new StringBuilder();

        sb.AppendLine(row0.TrimEnd(HorizontalJointTop) + RightTop);

        foreach (var (index, row) in rows.Index())
        {
            sb.AppendLine(row);

            if (index < rows.Count - 1)
                sb.AppendLine(row2.TrimEnd(CentreJoint) + VerticalJointRight);
        }

        sb.AppendLine(row3.TrimEnd(HorizontalJointBottom) + RightBottom);
        sb.AppendLine(row4);

        return sb.ToString();
    }

    private static string GetSquareSymbol(SquareStatus status, bool positioning, bool targeting)
    {
        if (positioning)
        {
            if (status.HasFlag(SquareStatus.AircraftCarrier)) return "a";
            if (status.HasFlag(SquareStatus.Battleship)) return "b";
            if (status.HasFlag(SquareStatus.Cruiser)) return "c";
            if (status.HasFlag(SquareStatus.Submarine) || status.HasFlag(SquareStatus.Ship)) return "s";
            if (status.HasFlag(SquareStatus.Destroyer)) return "d";
        }
        else if (targeting && status.HasFlag(SquareStatus.Hit))
        {
            return (status & SquareStatus.Ship) != 0 ? "h" : "m";
        }

        return " ";
    }

    internal static int LetterSequenceToInteger(string value)
    {
        int result = 0;

        foreach (char letter in value.ToUpperInvariant())
        {
            result *= Letters.Length;
            result += letter - 'A' + 1;
        }

        return result - 1;
    }

    internal static string IntegerToLetterSequence(int value)
    {
        string result = "";

        if (value < 0)
            return result;

        if (value >= Letters.Length)
            result += Letters[value / Letters.Length - 1];

        result += Letters[value % Letters.Length];

        return result;
    }
}
