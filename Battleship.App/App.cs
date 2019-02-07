using Battleship.Library.Enums;
using Battleship.Library.Models;
using Battleship.Library.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Battleship.App
{
    public class App
    {
        private readonly IGridService _gridService;

        private const char CellLeftTop = '┌';
        private const char CellRightTop = '┐';
        private const char CellLeftBottom = '└';
        private const char CellRightBottom = '┘';
        private const char CellHorizontalJointTop = '┬';
        private const char CellHorizontalJointBottom = '┴';
        private const char CellVerticalJointLeft = '├';
        private const char CellTJoint = '┼';
        private const char CellVerticalJointRight = '┤';
        private const char CellHorizontalLine = '─';
        private const char CellVerticalLine = '│';

        public App(IGridService gridService)
        {
            _gridService = gridService;
        }

        public void Start()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Lets play battleship!");

            int width, height;
            width = height = RequestInt("Please enter a grid size:");
            int shipCount = RequestInt("Please enter no of ships:");

            Grid playerGrid = _gridService.Create(width, height);
            Grid enemyGrid = _gridService.Create(width, height);

            _gridService.SetRandomShipPositions(enemyGrid, shipCount);

            bool autoPositionShips = RequestBool("Place ships randomly:");
            if (autoPositionShips)
            {
                _gridService.SetRandomShipPositions(playerGrid, shipCount);
                DrawGrid(playerGrid, true, false);
            }
            else
            {
                for (int i = 0; i < shipCount; i++)
                {
                    DrawGrid(playerGrid, true, false);

                    Square square = null;
                    while (square == null)
                    {
                        Console.WriteLine($"Ship {i}");

                        Point coords = RequestPoint($"Please enter ship {i} co-ords:");
                        square = _gridService.GetSquare(playerGrid, coords);

                        if (square == null)
                        {
                            Console.WriteLine($"{coords.X},{coords.Y} outside bounds of the grid");
                            continue;
                        }

                        if (square.Status != SquareStatus.Ship) continue;

                        Console.WriteLine($"Ship already at position {coords.X},{coords.Y}");
                        square = null;
                    }

                    square.Status = SquareStatus.Ship;
                }

                DrawGrid(playerGrid, true, false);
            }

            bool autoTargetShips = RequestBool("Target ships randomly:");

            var rand = new Random();
            bool playersTurn = rand.NextDouble() > 0.5;
            string currentPlayer = playersTurn ? "Player" : "Enemy";

            Console.WriteLine();
            Console.WriteLine($"{currentPlayer} starts.");

            bool inGame = true;
            while (inGame)
            {
                Console.WriteLine();
                Console.WriteLine($"{currentPlayer}'s turn.");

                Grid targetGrid = playersTurn ? enemyGrid : playerGrid;

                DrawGrid(targetGrid, false, true);
                List<Point> validTargets = _gridService.GetValidTargets(targetGrid).ToList();

                var selectedTarget = new Point();
                if (autoTargetShips || !playersTurn)
                {
                    selectedTarget = validTargets[rand.Next(0, validTargets.Count)];
                }
                else
                {
                    bool validTarget = false;
                    while (!validTarget)
                    {
                        selectedTarget = RequestPoint("Please enter target coordinates:");

                        if (validTargets.Contains(selectedTarget))
                        {
                            validTarget = true;
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine($"{selectedTarget.X},{selectedTarget.Y} isn't a valid target");
                        }
                    }
                }

                Console.WriteLine($"{currentPlayer} attacks {selectedTarget.X},{selectedTarget.Y}");

                bool hit = _gridService.Attack(targetGrid, selectedTarget);
                Console.WriteLine(hit ? "KABOOM! Attack successful!" : "Sploosh. Attack unsuccessful.");

                IEnumerable<Point> remainingShipPositions = _gridService.GetShipPositions(targetGrid);
                if (remainingShipPositions.Any())
                {
                    playersTurn = !playersTurn;
                    currentPlayer = playersTurn ? "Player" : "Enemy";
                    continue;
                }

                Console.WriteLine();
                Console.WriteLine($"{currentPlayer} wins!");
                inGame = false;
            }

            Console.WriteLine();
            Console.WriteLine("Thanks for playing!");
            Console.ReadLine();
        }

        private static string RequestString(string request)
        {
            Console.WriteLine();
            Console.WriteLine(request);
            return Console.ReadLine();
        }

        private static int RequestInt(string request)
        {
            string input = RequestString(request);
            return int.Parse(input);
        }

        private static bool RequestBool(string request)
        {
            string input = RequestString(request);
            return bool.Parse(input);
        }

        private static Point RequestPoint(string request)
        {
            string input = RequestString(request);

            string[] inputCoords = input.Split(',');
            int x = int.Parse(inputCoords[0]);
            int y = int.Parse(inputCoords[1]);

            return new Point(x, y);
        }

        private void DrawGrid(Grid grid, bool displayShips, bool displayResults)
        {
            int maxX = grid.Squares.GetLength(0);
            int maxY = grid.Squares.GetLength(1);

            var rows = new List<string>();

            string row0 = "  ";
            string row1 = $" {CellLeftTop}";
            string row2 = $"{CellVerticalLine}";
            string row3 = $" {CellVerticalJointLeft}";
            string row4 = $" {CellLeftBottom}";

            for (int x = 0; x < maxX; x++)
            {
                row0 += $"{x.ToString().Last()} ";
                row1 += $"{CellHorizontalLine}{CellHorizontalJointTop}";
                row2 += $" {CellVerticalLine}";
                row3 += $"{CellHorizontalLine}{CellTJoint}";
                row4 += $"{CellHorizontalLine}{CellHorizontalJointBottom}";
            }

            for (int y = 0; y < maxY; y++)
            {
                rows.Add(y.ToString().Last() + row2);
                rows.Add(row3);
            }

            rows.RemoveAt(rows.Count - 1);

            if (displayShips)
            {
                IEnumerable<Point> shipPositions = _gridService.GetShipPositions(grid);
                PopulateGrid(rows, shipPositions, 'S');
            }

            if (displayResults)
            {
                IEnumerable<Point> hitPositions = _gridService.GetHitPositions(grid);
                PopulateGrid(rows, hitPositions, 'M');

                IEnumerable<Point> deadShipPositions = _gridService.GetDeadShipPositions(grid);
                PopulateGrid(rows, deadShipPositions, 'H');
            }

            Console.WriteLine();
            Console.WriteLine(row0);
            Console.WriteLine(row1.TrimEnd(CellHorizontalJointTop) + CellRightTop);
            foreach (string row in rows)
            {
                if (row.EndsWith($"{CellTJoint}"))
                {
                    Console.WriteLine(row.TrimEnd(CellTJoint) + CellVerticalJointRight);
                }
                else
                {
                    Console.WriteLine(row);
                }
            }
            Console.WriteLine(row4.TrimEnd(CellHorizontalJointBottom) + CellRightBottom);
        }

        private static void PopulateGrid(IList<string> rows, IEnumerable<Point> points, char value)
        {
            foreach (Point point in points)
            {
                int rowIndex = point.Y * 2;
                int charIndex = (point.X + 1) * 2;

                var sb = new StringBuilder(rows[rowIndex]) { [charIndex] = value };
                rows[rowIndex] = sb.ToString();
            }
        }
    }
}
