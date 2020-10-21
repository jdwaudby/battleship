using System.Collections.Generic;
using System.Drawing;
using Battleship.Library.Enums;
using Battleship.Library.Models;

namespace Battleship.Library.Services.Interfaces
{
    public interface IGridService
    {
        Grid Create();
        Grid Create(int width, int height);
        void SetShipPosition(Grid grid, Ship ship);
        void SetShipPosition(Grid grid, Ship ship, Point position, Heading heading);
        IEnumerable<Point> GetShipPositions(Grid grid);
        IEnumerable<Point> GetValidTargets(Grid grid);
        bool Attack(Grid grid, Point target);
    }
}