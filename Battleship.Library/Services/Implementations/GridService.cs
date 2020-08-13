using Battleship.Library.Enums;
using Battleship.Library.Models;
using Battleship.Library.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Battleship.Library.Services.Implementations
{
    public class GridService : IGridService
    {
        public Grid Create()
        {
            const int size = 10;
            return Create(size, size);
        }
        
        public Grid Create(int width, int height)
        {
            var grid = new Grid(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid.Squares[x, y] = new Square();
                }
            }

            return grid;
        }

        public void SetShipPosition(Grid grid, Ship ship)
        {
            var random = new Random();

            int maxX = grid.Squares.GetLength(0);
            int maxY = grid.Squares.GetLength(1);

            var squares = new List<Square>();
            do
            {
                squares.Clear();
                
                int startX, startY;
                
                bool isHorizontal = random.Next(0, 2) > 0;
                if (isHorizontal)
                {
                    startX = random.Next(0, maxX - ship.Length);
                    startY = random.Next(0, maxY);
                }
                else
                {
                    startX = random.Next(0, maxX);
                    startY = random.Next(0, maxY - ship.Length);
                }

                for (int i = 0; i < ship.Length; i++)
                {
                    int x = startX;
                    int y = startY;

                    if (isHorizontal)
                        x += i;
                    else
                        y += i;
                    
                    squares.Add(grid.Squares[x, y]);
                }
            } while (squares.Any(x => x.Status.HasValue));

            foreach (Square square in squares)
                square.Status = (SquareStatus) Enum.Parse(typeof(SquareStatus), ship.Type.ToString());
        }

        public IEnumerable<Point> GetShipPositions(Grid grid)
        {
            return GetPositions(grid, SquareStatus.Ship);
        }

        public Square GetSquare(Grid grid, Point point)
        {
            int maxX = grid.Squares.GetLength(0);
            int maxY = grid.Squares.GetLength(1);

            if (point.X < maxX || point.Y < maxY)
                return grid.Squares[point.X, point.Y];

            return null;
        }

        public void SetRandomShipPositions(Grid grid, int ships)
        {
            int maxX = grid.Squares.GetLength(0);
            int maxY = grid.Squares.GetLength(1);

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
            var emptyPositions = GetEmptyPositions(grid);
            var shipPositions = GetShipPositions(grid);
            return emptyPositions.Concat(shipPositions);
        }

        public bool Attack(Grid grid, Point target)
        {
            Square square = grid.Squares[target.X, target.Y];
            if (square.Status.HasValue && SquareStatus.Ship.HasFlag(square.Status))
            {
                square.Status = SquareStatus.Hit;
                return true;
            }

            square.Status = SquareStatus.Miss;
            return false;
        }

        private static IEnumerable<Point> GetPositions(Grid grid, SquareStatus status)
        {
            var positions = new List<Point>();

            for (int x = 0; x < grid.Squares.GetLength(0); x++)
            {
                for (int y = 0; y < grid.Squares.GetLength(1); y++)
                {
                    var squareStatus = grid.Squares[x, y].Status;
                    if (squareStatus.HasValue && status.HasFlag(squareStatus))
                        positions.Add(new Point(x, y));
                }
            }

            return positions;
        }

        private static IEnumerable<Point> GetEmptyPositions(Grid grid)
        {
            var positions = new List<Point>();

            for (int x = 0; x < grid.Squares.GetLength(0); x++)
            {
                for (int y = 0; y < grid.Squares.GetLength(1); y++)
                {
                    if (grid.Squares[x, y].Status == null)
                        positions.Add(new Point(x, y));
                }
            }

            return positions;
        }
    }
}
