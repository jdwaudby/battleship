using Battleship.Library.Enums;

namespace Battleship.Library.Models;

public static class GridExtensions
{
    extension(Grid grid)
    {
        public bool HasShipsRemaining => grid.Squares.Any(s => s.Status.HasValue && SquareStatus.Ship.HasFlag(s.Status.Value));
    }
}
