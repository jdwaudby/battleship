using Battleship.Library.Enums;

namespace Battleship.Library.Models;

public class Ship(ShipType type = ShipType.Custom, int length = 1)
{
    public ShipType Type { get; set; } = type;

    public int Length
    {
        get;
        set => field = value > 0
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value), "Ship length must be positive.");
    } = length;
}
