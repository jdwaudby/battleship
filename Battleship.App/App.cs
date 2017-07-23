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
                    var shipPositions = _gridService.GetShipPositions(playerGrid);
                    foreach (Point shipPosition in shipPositions)
                    {
                        Console.WriteLine($"Ship at {shipPosition.X},{shipPosition.Y}");
                    }

                    Console.WriteLine($"Please enter ship {i} co-ords:");
                    input = Console.ReadLine();

                    var inputCoords = input.Split(',');
                    int x = int.Parse(inputCoords[0]);
                    int y = int.Parse(inputCoords[1]);

                    // Todo: Check co-ords are within the grid boundaries.

                    Square square = playerGrid.Squares[x, y];
                    while (square.Status == SquareStatus.Ship)
                    {
                        Console.WriteLine($"Ship already at position {x},{y}");
                        Console.WriteLine($"Please enter ship {i} co-ords:");
                        input = Console.ReadLine();

                        inputCoords = input.Split(',');
                        x = int.Parse(inputCoords[0]);
                        y = int.Parse(inputCoords[1]);

                        square = playerGrid.Squares[x, y];
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
    }
}
