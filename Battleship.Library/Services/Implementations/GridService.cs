﻿using System;
using System.Collections.Generic;
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
            return new Grid(width, height);
        }

        public void SetShipPosition(Grid grid, Ship ship)
        {
            var random = new Random();
            var headings = Enum.GetValues(typeof(Heading)).Cast<Heading>().ToArray();
            var emptySquares = grid.Squares.Where(square => square.Status == null).ToList();

            var squares = new List<Square>();
            do
            {
                squares.Clear();

                Square bowSquare = emptySquares[random.Next(emptySquares.Count)];
                var heading = (Heading)headings.GetValue(random.Next(headings.Length));
                switch (heading)
                {
                    case Heading.North:
                        squares = grid.Squares.Where(square => square.X == bowSquare.X).ToList();
                        break;
                    case Heading.South:
                        squares = grid.Squares.Where(square => square.X == bowSquare.X).Reverse().ToList();
                        break;
                    case Heading.East:
                        squares = grid.Squares.Where(square => square.Y == bowSquare.Y).Reverse().ToList();
                        break;
                    case Heading.West:
                        squares = grid.Squares.Where(square => square.Y == bowSquare.Y).ToList();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(heading), heading, null);
                }

                int index = squares.IndexOf(bowSquare);
                try
                {
                    squares = squares.GetRange(index, ship.Length);
                }
                catch (ArgumentException)
                {
                    squares.Clear();
                }
            } while (!squares.Any() || squares.Any(x => x.Status.HasValue));

            SquareStatus status;
            if (ship.Type == ShipType.Custom)
            {
                status = SquareStatus.Ship;
            }
            else
            {
                status = (SquareStatus)Enum.Parse(typeof(SquareStatus), ship.Type.ToString());
            }

            foreach (Square square in squares)
            {
                square.Status = status;
            }
        }

        public void SetShipPosition(Grid grid, Ship ship, string bowPosition, Heading heading)
        {
            Square bowSquare = grid.Squares.SingleOrDefault(square => square.Coordinates == bowPosition);
            if (bowSquare == null)
            {
                throw new ShipPositioningException($"Unable to find square at position {bowPosition}");
            }

            List<Square> squares;
            switch (heading)
            {
                case Heading.North:
                    squares = grid.Squares.Where(square => square.X == bowSquare.X).ToList();
                    break;
                case Heading.South:
                    squares = grid.Squares.Where(square => square.X == bowSquare.X).Reverse().ToList();
                    break;
                case Heading.East:
                    squares = grid.Squares.Where(square => square.Y == bowSquare.Y).Reverse().ToList();
                    break;
                case Heading.West:
                    squares = grid.Squares.Where(square => square.Y == bowSquare.Y).ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(heading), heading, null);
            }

            int index = squares.IndexOf(bowSquare);
            squares = squares.GetRange(index, ship.Length);

            SquareStatus status;
            if (ship.Type == ShipType.Custom)
            {
                status = SquareStatus.Ship;
            }
            else
            {
                status = (SquareStatus)Enum.Parse(typeof(SquareStatus), ship.Type.ToString());
            }

            foreach (Square square in squares)
            {
                square.Status = status;
            }
        }

        public IEnumerable<string> GetValidTargets(Grid grid)
        {
            var emptyPositions = GetEmptyPositions(grid);
            var shipPositions = GetShipPositions(grid);
            return emptyPositions.Concat(shipPositions);
        }

        public IEnumerable<string> GetShipPositions(Grid grid)
        {
            return GetPositions(grid, SquareStatus.Ship);
        }

        public bool Attack(Grid grid, string target)
        {
            var validTargets = GetValidTargets(grid);
            if (!validTargets.Contains(target))
            {
                throw new ShipTargetingException("Invalid target");
            }

            Square square = grid.Squares.SingleOrDefault(x => x.Coordinates == target);
            if (square == null)
            {
                throw new ShipTargetingException($"Unable to find square at position {target}");
            }

            if (square.Status.HasValue && SquareStatus.Ship.HasFlag(square.Status))
            {
                square.Status = SquareStatus.Hit;
                return true;
            }

            square.Status = SquareStatus.Miss;
            return false;
        }

        private static IEnumerable<string> GetPositions(Grid grid, SquareStatus status)
        {
            return grid.Squares.Where(square => square.Status.HasValue && status.HasFlag(square.Status))
                .Select(square => square.Coordinates);
        }

        private static IEnumerable<string> GetEmptyPositions(Grid grid)
        {
            return grid.Squares.Where(square => square.Status == null).Select(square => square.Coordinates);
        }
    }
}