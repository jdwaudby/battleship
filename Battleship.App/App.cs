using System;
using System.Drawing;
using System.Linq;
using Battleship.Library.Enums;
using Battleship.Library.Models;
using Battleship.Library.Services.Interfaces;

namespace Battleship.App
{
    public class App
    {
        private readonly IGridService _gridService;

        public App(IGridService gridService)
        {
            _gridService = gridService;
        }

        public void Start()
        {
            Console.WriteLine("Lets play battleship!");

            Console.WriteLine();
            Console.WriteLine("Please enter a grid size:");
            string input = Console.ReadLine();

            int width, height;
            width = height = int.Parse(input);

            Grid playerGrid = _gridService.Create(width, height);
            Grid enemyGrid = _gridService.Create(width, height);

            Console.WriteLine();
            Console.WriteLine("Please enter no of ships:");
            input = Console.ReadLine();

            int ships = int.Parse(input);

            _gridService.SetRandomShipPositions(enemyGrid, ships);

            Console.WriteLine();
            Console.WriteLine("Place ships randomly:");
            input = Console.ReadLine();

            bool autoPositionShips = bool.Parse(input);
            if (autoPositionShips)
            {
                _gridService.SetRandomShipPositions(playerGrid, ships);
            }
            else
            {
                for (int i = 0; i < ships; i++)
                {
                    Console.WriteLine();

                    var shipPositions = _gridService.GetShipPositions(playerGrid);
                    foreach (Point shipPosition in shipPositions)
                    {                        
                        Console.WriteLine($"Ship at {shipPosition.X},{shipPosition.Y}");
                    }

                    Square square = null;
                    while (square == null)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Ship {i}");

                        Point coords = RequestCoordinates();
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
            }

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

                var validTargets = _gridService.GetValidTargets(targetGrid).ToList();

                // Todo: Allow player to select their own target.
                Point selectedTarget = validTargets[rand.Next(0, validTargets.Count)];

                Console.WriteLine($"{currentPlayer} attacks {selectedTarget.X},{selectedTarget.Y}");

                bool hit = _gridService.Attack(targetGrid, selectedTarget);
                Console.WriteLine(hit ? "KABOOM! Attack successful!" : "Sploosh. Attack unsuccessful.");

                var remainingShipPositions = _gridService.GetShipPositions(targetGrid);
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

        private static Point RequestCoordinates()
        {
            Console.WriteLine("Please enter ship co-ords:");
            string input = Console.ReadLine();

            var inputCoords = input.Split(',');
            int x = int.Parse(inputCoords[0]);
            int y = int.Parse(inputCoords[1]);

            return new Point(x, y);
        }
    }
}
