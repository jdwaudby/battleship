namespace Battleship.Library.Enums
{
    [Flags]
    public enum SquareStatus
    {
        AircraftCarrier = 1 << 0,
        Battleship = 1 << 1,
        Cruiser = 1 << 2,
        Submarine = 1 << 3,
        Destroyer = 1 << 4,
        Hit = 1 << 5,

        Ship = AircraftCarrier | Battleship | Cruiser | Submarine | Destroyer
    }
}
