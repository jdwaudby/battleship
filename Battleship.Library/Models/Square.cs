using Battleship.Library.Enums;

namespace Battleship.Library.Models;

public class Square(string y, int x)
{
    public string Y { get; } = y;
    public string X { get; } = x.ToString();
    public string Coordinates => $"{Y}{X}";
    public SquareStatus? Status { get; set; }
}
