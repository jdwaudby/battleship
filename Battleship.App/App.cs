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
        private readonly IShipService _shipService;

        public App(IGridService gridService, IShipService shipService)
        {
            _gridService = gridService;
            _shipService = shipService;
        }

        public void Start()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Lets play battleship!");

            do
            {
                string input = RequestString("Do you want to play a standard or custom game?");
                if (input.Equals("standard", StringComparison.OrdinalIgnoreCase))
                    PlayStandardGame();
                else
                    PlayCustomGame();
            } while (RequestBool("Do you want to play again?"));

            Console.WriteLine();
            Console.WriteLine("Thanks for playing!");
            Console.ReadLine();
        }

        private void PlayStandardGame()
        {
            Grid playerGrid = _gridService.Create();
            Grid enemyGrid = _gridService.Create();

            var playerShips = _shipService.Get();
            var enemyShips = _shipService.Get();

            foreach (Ship enemyShip in enemyShips)
            {
                _gridService.SetShipPosition(enemyGrid, enemyShip);
            }
            
            bool autoPositionShips = RequestBool("Position ships randomly:");
            if (autoPositionShips)
            {
                foreach (Ship playerShip in playerShips)
                {
                    _gridService.SetShipPosition(playerGrid, playerShip);
                }

                Console.WriteLine();
                Console.WriteLine("{0:Positioning}", playerGrid);
            }
            else
            {
                foreach (Ship playerShip in playerShips)
                {
                    Console.WriteLine();
                    Console.WriteLine("{0:Positioning}", playerGrid);
                    
                    // Todo: Manually position ship
                }
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

                Console.WriteLine();
                Console.WriteLine("{0:Targeting}", targetGrid);

                var validTargets = _gridService.GetValidTargets(targetGrid).ToList();

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
        }
        
        private void PlayCustomGame()
        {
            int width = RequestInt("Please enter grid width:");
            int height = RequestInt("Please enter grid height:");
            int shipCount = RequestInt("Please enter no of ships:");

            Grid playerGrid = _gridService.Create(width, height);
            Grid enemyGrid = _gridService.Create(width, height);

            _gridService.SetRandomShipPositions(enemyGrid, shipCount);

            bool autoPositionShips = RequestBool("Place ships randomly:");
            if (autoPositionShips)
            {
                _gridService.SetRandomShipPositions(playerGrid, shipCount);

                Console.WriteLine();
                Console.WriteLine("{0:Positioning}", playerGrid);
            }
            else
            {
                for (int i = 0; i < shipCount; i++)
                {
                    Console.WriteLine();
                    Console.WriteLine("{0:Positioning}", playerGrid);

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

                        if (square.Status != SquareStatus.Ship)
                        {
                            continue;
                        }

                        Console.WriteLine($"Ship already at position {coords.X},{coords.Y}");
                        square = null;
                    }

                    square.Status = SquareStatus.Ship;
                }

                Console.WriteLine();
                Console.WriteLine("{0:Positioning}", playerGrid);
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

                Console.WriteLine();
                Console.WriteLine("{0:Targeting}", targetGrid);

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
    }
}
