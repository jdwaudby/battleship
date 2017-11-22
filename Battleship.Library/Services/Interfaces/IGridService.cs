using Battleship.Library.Models;
using System.Collections.Generic;
using System.Drawing;

namespace Battleship.Library.Services.Interfaces
{
    public interface IGridService
    {
        Grid Create(int width, int height);
        IEnumerable<Point> GetShipPositions(Grid grid);
        Square GetSquare(Grid grid, Point point);
        void SetRandomShipPositions(Grid grid, int ships);
        IEnumerable<Point> GetValidTargets(Grid grid);
        bool Attack(Grid grid, Point target);
        IEnumerable<Point> GetHitPositions(Grid grid);
        IEnumerable<Point> GetDeadShipPositions(Grid grid);
    }
}
