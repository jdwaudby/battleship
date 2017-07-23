using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Battleship.Library.Enums;
using Battleship.Library.Models;
using Battleship.Library.Services.Interfaces;

namespace Battleship.Library.Services.Implementations
{
    public class GridService : IGridService
    {
        private readonly ISquareService _squareService;

        public GridService(ISquareService squareService)
        {
            _squareService = squareService;
        }

        public Grid Create(int width, int height)
        {
            var grid = new Grid(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid.Squares[x, y] = _squareService.Create();
                }
            }

            return grid;
        }

        public IEnumerable<Point> GetShipPositions(Grid grid)
        {
            return GetPositions(grid, SquareStatus.Ship);
        }

        public void SetRandomShipPositions(Grid grid, int maxX, int maxY, int ships)
        {
            var rand = new Random();
            for (int i = 0; i < ships; i++)
            {
                int x = rand.Next(0, maxX), y = rand.Next(0, maxY);

                Square square = grid.Squares[x, y];
                while (square.Status == SquareStatus.Ship)
                {
                    x = rand.Next(0, maxX);
                    y = rand.Next(0, maxY);
                    square = grid.Squares[x, y];
                }

                square.Status = SquareStatus.Ship;
            }
        }

        public IEnumerable<Point> GetValidTargets(Grid grid)
        {
            return GetPositions(grid, SquareStatus.Empty, SquareStatus.Ship);
        }

        public bool Attack(Grid grid, Point target)
        {
            Square square = grid.Squares[target.X, target.Y];
            if (square.Status == SquareStatus.Ship)
            {
                square.Status = SquareStatus.DeadShip;
                return true;
            }

            square.Status = SquareStatus.Hit;
            return false;
        }

        private static IEnumerable<Point> GetPositions(Grid grid, params SquareStatus[] statuses)
        {
            var positions = new List<Point>();

            for (int x = 0; x < grid.Squares.GetLength(0); x++)
            {
                for (int y = 0; y < grid.Squares.GetLength(1); y++)
                {
                    if (statuses.Contains(grid.Squares[x, y].Status))
                        positions.Add(new Point(x, y));
                }
            }

            return positions;
        }
    }
}
