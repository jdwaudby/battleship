using Battleship.Library;
using Battleship.Library.Enums;
using System;
using System.Diagnostics;

namespace Battleship.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Lets play battleship!");

            Console.WriteLine("Please enter a grid size:");
            string input = Console.ReadLine();

            int width, height;
            width = height = int.Parse(input);

            var playerGrid = new Grid(width, height);
            var enemyGrid = new Grid(width, height);

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