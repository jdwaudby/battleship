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

        public IEnumerable<string> UnsunkHitPositions =>
            grid.Squares
                .Where(s => s.Status is { } status
                            && status.HasFlag(SquareStatus.Hit)
                            && (status & SquareStatus.Ship) != 0
                            && !grid.HasShipBeenSunk(status.ToShipType()))
                .OrderBy(s => s.LastUpdated)
                .Select(s => s.Coordinates);

        public bool IsValidTarget(string coordinates) =>
            grid[coordinates] is { } square && (square.Status is null || SquareStatus.Ship.HasFlag(square.Status.Value));

        public IEnumerable<string> GetValidNeighbouringTargets(string position, string y = "", string x = "")
        {
            Square middle = grid[position] ?? throw new InvalidOperationException($"Unable to find square at position {position}");

            List<Square?> candidates = [];

            if (string.IsNullOrEmpty(y))
            {
                int yInt = Grid.LetterSequenceToInteger(middle.Y);
                candidates.Add(grid[$"{Grid.IntegerToLetterSequence(yInt + 1)}{middle.X}"]);
                candidates.Add(grid[$"{Grid.IntegerToLetterSequence(yInt - 1)}{middle.X}"]);
            }

            if (string.IsNullOrEmpty(x))
            {
                int xInt = int.Parse(middle.X);
                candidates.Add(grid[$"{middle.Y}{xInt - 1}"]);
                candidates.Add(grid[$"{middle.Y}{xInt + 1}"]);
            }

            return candidates
                .OfType<Square>()
                .Where(s => grid.IsValidTarget(s.Coordinates))
                .Select(s => s.Coordinates);
        }

        public IEnumerable<string> GetPositions(SquareStatus status) =>
            grid.Squares.Where(s => s.Status.HasValue && status.HasFlag(s.Status.Value))
                .Select(s => s.Coordinates);

        public bool HasShipBeenSunk(ShipType shipType) =>
            shipType is ShipType.Custom || !grid.GetPositions(shipType.ToSquareStatus()).Any();

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
