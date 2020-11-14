using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Battleship.Library.Enums;
using Battleship.Library.Exceptions;
using Battleship.Library.Models;
using Battleship.Library.Services.Interfaces;

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
                Grid playerGrid, enemyGrid;

                var gameType = RequestEnum<GameType>("Do you want to play a standard or custom game?");
                switch (gameType)
                {
                    case GameType.Standard:
                        playerGrid = _gridService.Create();
                        enemyGrid = _gridService.Create();

                        SetUpStandardGame(playerGrid, enemyGrid);
                        break;
                    case GameType.Custom:
                        int width = RequestInt("Please enter grid width:");
                        int height = RequestInt("Please enter grid height:");

                        playerGrid = _gridService.Create(width, height);
                        enemyGrid = _gridService.Create(width, height);

                        SetUpCustomGame(playerGrid, enemyGrid);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                PlayGame(playerGrid, enemyGrid);
            } while (RequestBool("Do you want to play again?"));

            Console.WriteLine();
            Console.WriteLine("Thanks for playing!");
            Console.ReadLine();
        }

        private void SetUpStandardGame(Grid playerGrid, Grid enemyGrid)
        {
            var playerShips = _shipService.Get();
            var enemyShips = _shipService.Get();

            bool autoPositionShips = RequestBool("Position ships randomly:");
            if (autoPositionShips)
            {
                foreach (Ship playerShip in playerShips)
                    _gridService.SetShipPosition(playerGrid, playerShip);
            }
            else
            {
                foreach (Ship playerShip in playerShips)
                    SetShipPositionManually(playerGrid, playerShip);
            }

            foreach (Ship enemyShip in enemyShips)
            {
                _gridService.SetShipPosition(enemyGrid, enemyShip);
            }

            Console.WriteLine();
            Console.WriteLine("{0:Positioning}", playerGrid);
        }

        private void SetUpCustomGame(Grid playerGrid, Grid enemyGrid)
        {
            int shipCount = RequestInt("Please enter no of ships:");

            var playerShips = new List<CustomShip>();
            for (int i = 0; i < shipCount; i++)
            {
                playerShips.Add(new CustomShip(1));

                var enemyShip = new CustomShip(1);
                _gridService.SetShipPosition(enemyGrid, enemyShip);
            }

            bool autoPositionShips = RequestBool("Position ships randomly:");
            if (autoPositionShips)
            {
                foreach (CustomShip playerShip in playerShips)
                    _gridService.SetShipPosition(playerGrid, playerShip);
            }
            else
            {
                foreach (CustomShip playerShip in playerShips)
                    SetShipPositionManually(playerGrid, playerShip);
            }

            Console.WriteLine();
            Console.WriteLine("{0:Positioning}", playerGrid);
        }

        private void SetShipPositionManually(Grid grid, Ship ship)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine("{0:Positioning}", grid);

                Console.WriteLine($"Ship {ship.Type}, length {ship.Length}");

                string coords = RequestString("Please enter bow co-ords:");
                var heading = RequestEnum<Heading>("Please enter a heading:");

                _gridService.SetShipPosition(grid, ship, coords, heading);
            }
            catch (ShipPositioningException e)
            {
                Console.WriteLine(e.Message);
                SetShipPositionManually(grid, ship);
            }
        }

        private void PlayGame(Grid playerGrid, Grid enemyGrid)
        {
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

                string selectedTarget = "";
                if (autoTargetShips || !playersTurn)
                {
                    selectedTarget = validTargets[rand.Next(0, validTargets.Count)];
                }
                else
                {
                    bool validTarget = false;
                    while (!validTarget)
                    {
                        selectedTarget = RequestString("Please enter target coordinates:");

                        if (validTargets.Contains(selectedTarget))
                        {
                            validTarget = true;
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine($"{selectedTarget} isn't a valid target");
                        }
                    }
                }

                Console.WriteLine($"{currentPlayer} attacks {selectedTarget}");

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

        private static T RequestEnum<T>(string request) where T : struct
        {
            string input = RequestString(request);
            return Enum.Parse<T>(input, true);
        }
    }
}