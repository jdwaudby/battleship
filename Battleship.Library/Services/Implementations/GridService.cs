using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Battleship.Library.Enums;
using Battleship.Library.Exceptions;
using Battleship.Library.Models;
using Battleship.Library.Services.Interfaces;

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
            Array headings = Enum.GetValues(typeof(Heading));

            int maxX = grid.Squares.GetLength(0);
            int maxY = grid.Squares.GetLength(1);

            var squares = new List<Square>();
            do
            {
                squares.Clear();
                
                int startX, startY;

                var heading = (Heading) headings.GetValue(random.Next(headings.Length));
                int xMultiplier = 0;
                int yMultiplier = 0;
                switch (heading)
                {
                    case Heading.North:
                        startX = random.Next(0, maxX);
                        startY = random.Next(0, maxY - ship.Length);
                        yMultiplier = 1;
                        break;
                    case Heading.South:
                        startX = random.Next(0, maxX);
                        startY = random.Next(ship.Length, maxY);
                        yMultiplier = -1;
                        break;
                    case Heading.East:
                        startX = random.Next(ship.Length, maxX);
                        startY = random.Next(0, maxY);
                        xMultiplier = -1;
                        break;
                    case Heading.West:
                        startX = random.Next(0, maxX - ship.Length);
                        startY = random.Next(0, maxY);
                        xMultiplier = 1;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                for (int i = 0; i < ship.Length; i++)
                {
                    int x = startX + i * xMultiplier;
                    int y = startY + i * yMultiplier;

                    squares.Add(grid.Squares[x, y]);
                }
            } while (squares.Any(x => x.Status.HasValue));

            foreach (Square square in squares)
            {
                if (ship.Type == ShipType.Custom)
                {
                    square.Status = SquareStatus.Ship;
                    continue;
                }
                    
                square.Status = (SquareStatus) Enum.Parse(typeof(SquareStatus), ship.Type.ToString());
            }
        }

        public void SetShipPosition(Grid grid, Ship ship, Point position, Heading heading)
        {
            int xMultiplier = 0;
            int yMultiplier = 0;
            switch (heading)
            {
                case Heading.North:
                    yMultiplier = 1;
                    break;
                case Heading.South:
                    yMultiplier = -1;
                    break;
                case Heading.East:
                    xMultiplier = -1;
                    break;
                case Heading.West:
                    xMultiplier = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            int maxX = grid.Squares.GetLength(0);
            int maxY = grid.Squares.GetLength(1);
            
            var squares = new List<Square>();
            for (int i = 0; i < ship.Length; i++)
            {
                int x = position.X + i * xMultiplier;
                int y = position.Y + i * yMultiplier;
                
                if (x < 0 || x >= maxX || y < 0 || y >= maxY)
                    throw new ShipPositioningException("Position outside grid");
                
                squares.Add(grid.Squares[x, y]);
            }

            if (squares.Any(x => x.Status.HasValue))
                throw new ShipPositioningException("Position already occupied");

            foreach (Square square in squares)
            {
                if (ship.Type == ShipType.Custom)
                {
                    square.Status = SquareStatus.Ship;
                    continue;
                }
                    
                square.Status = (SquareStatus) Enum.Parse(typeof(SquareStatus), ship.Type.ToString());
            }
        }

        public IEnumerable<Point> GetShipPositions(Grid grid)
        {
            return GetPositions(grid, SquareStatus.Ship);
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
