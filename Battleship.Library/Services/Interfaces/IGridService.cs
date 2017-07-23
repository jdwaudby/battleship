﻿using System.Collections.Generic;
using System.Drawing;
using Battleship.Library.Models;

namespace Battleship.Library.Services.Interfaces
{
    public interface IGridService
    {
        Grid Create(int width, int height);
        IEnumerable<Point> GetShipPositions(Grid grid);
        void SetRandomShipPositions(Grid grid, int maxX, int maxY, int ships);
        IEnumerable<Point> GetValidTargets(Grid grid);
        bool Attack(Grid grid, Point target);
    }
}