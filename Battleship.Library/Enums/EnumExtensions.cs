namespace Battleship.Library.Enums;

public static class EnumExtensions
{
    public static ShipType ToShipType(this SquareStatus status) => status switch
    {
        SquareStatus.AircraftCarrier => ShipType.AircraftCarrier,
        SquareStatus.Battleship      => ShipType.Battleship,
        SquareStatus.Cruiser         => ShipType.Cruiser,
        SquareStatus.Submarine       => ShipType.Submarine,
        SquareStatus.Destroyer       => ShipType.Destroyer,
        _                            => ShipType.Custom
    };

    public static SquareStatus ToSquareStatus(this ShipType shipType) => shipType switch
    {
        ShipType.AircraftCarrier => SquareStatus.AircraftCarrier,
        ShipType.Battleship      => SquareStatus.Battleship,
        ShipType.Cruiser         => SquareStatus.Cruiser,
        ShipType.Submarine       => SquareStatus.Submarine,
        ShipType.Destroyer       => SquareStatus.Destroyer,
        ShipType.Custom          => SquareStatus.Ship,
        _ => throw new ArgumentOutOfRangeException(nameof(shipType), shipType, null)
    };
}
