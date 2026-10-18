using Battleship.Library.Enums;

namespace Battleship.Library.Models;

public static class GridExtensions
{
    extension(Grid grid)
    {
        public bool HasShipsRemaining =>
            grid.Squares.Any(s => s.Status.HasValue && SquareStatus.Ship.HasFlag(s.Status.Value));

        public Square? GetSquare(string coordinates) => grid[coordinates];

        public IEnumerable<Square> EmptySquares =>
            grid.Squares.Where(s => s.Status is null);

        public IEnumerable<string> EmptyPositions =>
            grid.EmptySquares.Select(s => s.Coordinates);

        public IEnumerable<string> ShipPositions =>
            grid.GetPositions(SquareStatus.Ship);

        public IEnumerable<string> ValidTargets =>
            [.. grid.EmptyPositions, .. grid.ShipPositions];

        public bool IsValidTarget(string coordinates) =>
            grid[coordinates] is { } square && (square.Status is null || SquareStatus.Ship.HasFlag(square.Status.Value));

        public IEnumerable<string> GetPositions(SquareStatus status) =>
            grid.Squares.Where(s => s.Status.HasValue && status.HasFlag(s.Status.Value))
                .Select(s => s.Coordinates);

        public bool HasShipBeenSunk(ShipType shipType) =>
            !grid.GetPositions(shipType.ToSquareStatus()).Any();

        public IEnumerable<Square> GetSquares(Square bowSquare, Heading heading) => heading switch
        {
            Heading.North => grid.Squares.Where(square => square.X == bowSquare.X),
            Heading.South => grid.Squares.Where(square => square.X == bowSquare.X).Reverse(),
            Heading.East  => grid.Squares.Where(square => square.Y == bowSquare.Y).Reverse(),
            Heading.West  => grid.Squares.Where(square => square.Y == bowSquare.Y),
            _ => throw new ArgumentOutOfRangeException(nameof(heading), heading, null)
        };
    }
}
