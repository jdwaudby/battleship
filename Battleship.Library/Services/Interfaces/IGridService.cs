using Battleship.Library.Models;
using System.Collections.Generic;
using System.Drawing;

namespace Battleship.Library.Services.Interfaces
{
    public interface IGridService
    {
        Grid Create();
        Grid Create(int width, int height);
        void SetShipPosition(Grid grid, Ship ship);
        IEnumerable<Point> GetShipPositions(Grid grid);
        Square GetSquare(Grid grid, Point point);
        IEnumerable<Point> GetValidTargets(Grid grid);
        bool Attack(Grid grid, Point target);
    }
}
