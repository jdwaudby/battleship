﻿using Battleship.Library.Enums;
using Battleship.Library.Exceptions;
using Battleship.Library.Models;
using Battleship.Library.Services.Interfaces;

namespace Battleship.Library.Services.Implementations
{
    public class GridService : IGridService
    {
        private const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

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
            var random = new Random();
            var headings = Enum.GetValues(typeof(Heading)).Cast<Heading>().ToArray();
            var emptySquares = grid.Squares.Where(square => square.Status is null).ToList();

            var squares = new List<Square>();
            do
            {
                squares.Clear();

                Square bowSquare = emptySquares[random.Next(emptySquares.Count)];
                Heading heading = headings[random.Next(headings.Length)];
                squares = GetSquares(grid, bowSquare, heading).ToList();
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
            if (ship.Type is ShipType.Custom)
            {
                status = SquareStatus.Ship;
            }
            else
            {
                status = (SquareStatus)Enum.Parse(typeof(SquareStatus), ship.Type.ToString());
            }

            foreach (Square square in squares)
            {
                square.UpdateStatus(status);
            }
        }

        public void SetShipPosition(Grid grid, Ship ship, string bowPosition, Heading heading)
        {
            Square? bowSquare = grid.Squares.SingleOrDefault(square => square.Coordinates == bowPosition);
            if (bowSquare is null)
            {
                throw new ShipPositioningException($"Unable to find square at position {bowPosition}");
            }

            var squares = GetSquares(grid, bowSquare, heading).ToList();
            int index = squares.IndexOf(bowSquare);
            squares = squares.GetRange(index, ship.Length);

            SquareStatus status;
            if (ship.Type is ShipType.Custom)
            {
                status = SquareStatus.Ship;
            }
            else
            {
                status = (SquareStatus)Enum.Parse(typeof(SquareStatus), ship.Type.ToString());
            }

            foreach (Square square in squares)
            {
                square.UpdateStatus(status);
            }
        }

        public IEnumerable<string> GetValidTargets(Grid grid)
        {
            var emptyPositions = GetEmptyPositions(grid);
            var shipPositions = GetShipPositions(grid);
            return emptyPositions.Concat(shipPositions);
        }

        public IEnumerable<string> GetValidNeighbouringTargets(Grid grid, string position)
        {
            var middle = grid.Squares.Single(square => square.Coordinates == position);

            var squares = new List<Square>();

            var xInt = int.Parse(middle.X);
            var yInt = LetterSequenceToInteger(middle.Y);

            var square1 = grid.Squares.SingleOrDefault(square => square.X == middle.X && square.Y == IntegerToLetterSequence(yInt + 1));
            var square2 = grid.Squares.SingleOrDefault(square => square.X == middle.X && square.Y == IntegerToLetterSequence(yInt - 1));
            var square3 = grid.Squares.SingleOrDefault(square => square.X == (xInt - 1).ToString() && square.Y == middle.Y);
            var square4 = grid.Squares.SingleOrDefault(square => square.X == (xInt + 1).ToString() && square.Y == middle.Y);

            if (square1 is not null)
            {
                squares.Add(square1);
            }
            
            if (square2 is not null)
            {
                squares.Add(square2);
            }
            
            if (square3 is not null)
            {
                squares.Add(square3);
            }
            
            if (square4 is not null)
            {
                squares.Add(square4);
            }

            return squares.Where(square => square.Status is null || SquareStatus.Ship.HasFlag(square.Status.Value)).Select(square => square.Coordinates);
        }

        public IEnumerable<string> GetShipPositions(Grid grid)
        {
            return GetPositions(grid, SquareStatus.Ship);
        }

        public ShipType? Attack(Grid grid, string target)
        {
            var validTargets = GetValidTargets(grid);
            if (!validTargets.Contains(target))
            {
                throw new ShipTargetingException("Invalid target");
            }

            Square? square = grid.Squares.SingleOrDefault(x => x.Coordinates == target);
            if (square is null)
            {
                throw new ShipTargetingException($"Unable to find square at position {target}");
            }

            ShipType? shipType = null;
            if (square.Status.HasValue && SquareStatus.Ship.HasFlag(square.Status.Value))
            {
                
                if (square.Status.Value == SquareStatus.Ship)
                {
                    shipType = ShipType.Custom;
                }
                else
                {
                    shipType = (ShipType)Enum.Parse(typeof(ShipType), square.Status.Value.ToString());
                }
            }

            square.UpdateStatus(SquareStatus.Hit);
            return shipType;
        }

        public bool HasShipBeenSunk(Grid grid, ShipType shipType)
        {
            if (shipType == ShipType.Custom)
            {
                return true;
            }

            var status = (SquareStatus)Enum.Parse(typeof(SquareStatus), shipType.ToString());
            return !GetPositions(grid, status).Any();
        }

        private static IEnumerable<Square> GetSquares(Grid grid, Square bowSquare, Heading heading)
        {
            return heading switch
            {
                Heading.North => grid.Squares.Where(square => square.X == bowSquare.X).ToList(),
                Heading.South => grid.Squares.Where(square => square.X == bowSquare.X).Reverse().ToList(),
                Heading.East => grid.Squares.Where(square => square.Y == bowSquare.Y).Reverse().ToList(),
                Heading.West => grid.Squares.Where(square => square.Y == bowSquare.Y).ToList(),
                _ => throw new ArgumentOutOfRangeException(nameof(heading), heading, null)
            };
        }

        private static IEnumerable<string> GetPositions(Grid grid, SquareStatus status)
        {
            return grid.Squares.Where(square => square.Status.HasValue && status.HasFlag(square.Status.Value))
                .Select(square => square.Coordinates);
        }

        private static IEnumerable<string> GetEmptyPositions(Grid grid)
        {
            return grid.Squares.Where(square => square.Status is null).Select(square => square.Coordinates);
        }

        private static int LetterSequenceToInteger(string value)
        {
            var result = 0;

            foreach (var letter in value)
            {
                result *= Letters.Length;
                result += letter - 'A' + 1;
            }

            return result - 1;
        }

        private static string IntegerToLetterSequence(int value)
        {
            string result = "";

            if (value < 0)
                return result;

            if (value >= Letters.Length)
                result += Letters[value / Letters.Length - 1];

            result += Letters[value % Letters.Length];

            return result;
        }
    }
}