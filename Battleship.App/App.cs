using System;
using System.Diagnostics;
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

            Console.WriteLine("Please enter a grid size:");
            string input = Console.ReadLine();

            int width, height;
            width = height = int.Parse(input);

            Grid playerGrid = _gridService.Create(width, height);
            Grid enemyGrid = _gridService.Create(width, height);

            Console.WriteLine("Please enter no of ships:");
            input = Console.ReadLine();

            int ships = int.Parse(input);

            for (int i = 0; i < ships; i++)
            {
                for (int squareX = 0; squareX < width; squareX++)
                {
                    for (int squareY = 0; squareY < height; squareY++)
                    {
                        if (playerGrid.Squares[squareX, squareY].Status == SquareStatus.Ship)
                            Console.WriteLine($"Ship at {squareX},{squareY}");
                    }
                }

                var rand = new Random();

                int playerShipX, playerShipY,
                    enemyShipX = rand.Next(0, width), enemyShipY = rand.Next(0, height);

                Console.WriteLine($"Please enter ship {i} x position:");
                input = Console.ReadLine();

                playerShipX = int.Parse(input);

                Console.WriteLine($"Please enter ship {i} y position:");
                input = Console.ReadLine();

                playerShipY = int.Parse(input);

                Debug.WriteLine($"Enemy ship {enemyShipX},{enemyShipY}");

                // Todo: Prevent ships from being placed on squares already occupied by a ship.

                playerGrid.Squares[playerShipX, playerShipY].Status =
                    enemyGrid.Squares[enemyShipX, enemyShipY].Status = SquareStatus.Ship;
            }

            // Todo: Play the game!
        }
    }
}
