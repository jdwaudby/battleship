using System;
using System.Collections.Generic;
using System.Drawing;
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
            var shipPositions = new List<Point>();

            for (int x = 0; x < grid.Squares.GetLength(0); x++)
            {
                for (int y = 0; y < grid.Squares.GetLength(1); y++)
                {
                    if (grid.Squares[x,y].Status == SquareStatus.Ship)
                        shipPositions.Add(new Point(x, y));
                }
            }

            return shipPositions;
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
    }
}
