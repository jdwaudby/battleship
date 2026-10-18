using Battleship.Library.Enums;
using Battleship.Library.Exceptions;
using Battleship.Library.Models;
using Battleship.Library.Services.Interfaces;

namespace Battleship.Library.Services.Implementations;

public class GridService : IGridService
{
    public Grid Create()
    {
        const int size = 10;
        return Create(size, size);
    }

    public Grid Create(int width, int height)
    {
        return new(width, height);
    }

    public void SetShipPosition(Grid grid, Ship ship)
    {
        var headings = Enum.GetValues<Heading>();
        var emptySquares = grid.EmptySquares.ToList();

        List<Square> squares = [];
        do
        {
            squares.Clear();

            Square bowSquare = emptySquares[Random.Shared.Next(emptySquares.Count)];
            Heading heading = headings[Random.Shared.Next(headings.Length)];
            squares = [.. grid.GetSquares(bowSquare, heading)];
            int index = squares.IndexOf(bowSquare);
            try
            {
                squares = squares.GetRange(index, ship.Length);
            }
            catch (ArgumentException)
            {
                squares.Clear();
            }
        } while (squares.Count == 0 || squares.Any(x => x.Status.HasValue));

        SquareStatus status = ship.Type.ToSquareStatus();

        foreach (Square square in squares)
        {
            square.Status = status;
        }
    }

    public void SetShipPosition(Grid grid, Ship ship, string bowPosition, Heading heading)
    {
        Square? bowSquare = grid[bowPosition];
        if (bowSquare is null)
        {
            throw new ShipPositioningException($"Unable to find square at position {bowPosition}");
        }

        var squares = grid.GetSquares(bowSquare, heading).ToList();
        int index = squares.IndexOf(bowSquare);
        squares = squares.GetRange(index, ship.Length);

        SquareStatus status = ship.Type.ToSquareStatus();

        foreach (Square square in squares)
        {
            square.Status = status;
        }
    }

    public IEnumerable<string> GetValidTargets(Grid grid) => grid.ValidTargets;

    public IEnumerable<string> GetShipPositions(Grid grid) => grid.ShipPositions;

    public ShipType? Attack(Grid grid, string target)
    {
        if (!grid.IsValidTarget(target))
        {
            throw new ShipTargetingException("Invalid target");
        }

        Square? square = grid[target];
        if (square is null)
        {
            throw new ShipTargetingException($"Unable to find square at position {target}");
        }

        if (square.Status.HasValue && SquareStatus.Ship.HasFlag(square.Status.Value))
        {
            var shipType = square.Status.Value.ToShipType();
            square.Status = SquareStatus.Hit;
            return shipType;
        }

        square.Status = SquareStatus.Miss;
        return null;
    }

    public bool HasShipBeenSunk(Grid grid, ShipType shipType) => grid.HasShipBeenSunk(shipType);
}