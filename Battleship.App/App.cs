using System.Buffers;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Battleship.Library.Enums;
using Battleship.Library.Exceptions;
using Battleship.Library.Models;
using Battleship.Library.Services.Interfaces;

namespace Battleship.App;

public partial class App(IGridService gridService, IShipService shipService)
{
    private static readonly SearchValues<char> GridCharacters = SearchValues.Create("abcdhms");

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
                    playerGrid = gridService.Create();
                    enemyGrid = gridService.Create();

                    SetUpStandardGame(playerGrid, enemyGrid);
                    break;
                case GameType.Custom:
                    int width = RequestInt("Please enter grid width:");
                    int height = RequestInt("Please enter grid height:");

                    playerGrid = gridService.Create(width, height);
                    enemyGrid = gridService.Create(width, height);

                    SetUpCustomGame(playerGrid, enemyGrid);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            PlayGame(playerGrid, enemyGrid);
        } while (RequestBool("Do you want to play again?"));

        Console.WriteLine();
        Console.WriteLine("Thanks for playing!");
        WaitForKeyPress();
    }

    private void SetUpStandardGame(Grid playerGrid, Grid enemyGrid)
    {
        var playerShips = shipService.Get();
        var enemyShips = shipService.Get();

        bool autoPositionShips = RequestBool("Position ships randomly:");
        if (autoPositionShips)
        {
            foreach (Ship playerShip in playerShips)
            {
                gridService.SetShipPosition(playerGrid, playerShip);
            }
        }
        else
        {
            foreach (Ship playerShip in playerShips)
            {
                SetShipPositionManually(playerGrid, playerShip);
            }
        }

        foreach (Ship enemyShip in enemyShips)
        {
            gridService.SetShipPosition(enemyGrid, enemyShip);
        }

        DrawPositioningGrid(playerGrid);
    }

    private void SetUpCustomGame(Grid playerGrid, Grid enemyGrid)
    {
        int shipCount = RequestInt("Please enter no of ships:");

        List<CustomShip> playerShips = [];
        for (int i = 0; i < shipCount; i++)
        {
            playerShips.Add(new CustomShip(1));

            var enemyShip = new CustomShip(1);
            gridService.SetShipPosition(enemyGrid, enemyShip);
        }

        bool autoPositionShips = RequestBool("Position ships randomly:");
        if (autoPositionShips)
        {
            foreach (CustomShip playerShip in playerShips)
            {
                gridService.SetShipPosition(playerGrid, playerShip);
            }
        }
        else
        {
            foreach (CustomShip playerShip in playerShips)
            {
                SetShipPositionManually(playerGrid, playerShip);
            }
        }

        DrawPositioningGrid(playerGrid);
    }

    private void SetShipPositionManually(Grid grid, Ship ship)
    {
        try
        {
            DrawPositioningGrid(grid);

            Console.WriteLine($"Ship {ship.Type}, length {ship.Length}");

            string coords = RequestString("Please enter bow co-ords:");
            var heading = RequestEnum<Heading>("Please enter a heading:");

            gridService.SetShipPosition(grid, ship, coords, heading);
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
        int millisecondsTimeout = RequestInt("Please enter how many seconds to wait between actions:") * 1000;
        bool showContinuePrompt = millisecondsTimeout > 0;

        bool playersTurn = Random.Shared.NextDouble() > 0.5;
        string currentPlayer = playersTurn ? "Player" : "Enemy";
        int turnNumber = 0;

        var playerTracker = new TargetTracker();
        var enemyTracker = new TargetTracker();

        Console.WriteLine();
        Console.WriteLine($"{currentPlayer} starts.");

        bool inGame = true;
        while (inGame)
        {
            int roundNumber = turnNumber / 2 + 1;

            Grid targetGrid = playersTurn ? enemyGrid : playerGrid;
            TargetTracker currentTracker = playersTurn ? playerTracker : enemyTracker;

            DrawTargetingGrid(targetGrid, roundNumber, currentPlayer, showContinuePrompt, showHeader: true);

            Thread.Sleep(millisecondsTimeout);

            string selectedTarget = "";
            if (autoTargetShips || !playersTurn)
            {
                selectedTarget = currentTracker.SelectTarget(targetGrid, gridService);
            }
            else
            {
                bool validTarget = false;
                while (!validTarget)
                {
                    selectedTarget = RequestString("Please enter target coordinates:");

                    if (targetGrid.IsValidTarget(selectedTarget))
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
            Thread.Sleep(millisecondsTimeout);

            var shipType = gridService.Attack(targetGrid, selectedTarget);

            DrawTargetingGrid(targetGrid, roundNumber, currentPlayer, showContinuePrompt, showHeader: false);

            Console.WriteLine($"{currentPlayer} attacks {selectedTarget}");

            if (shipType is not null)
            {
                Console.WriteLine("KABOOM! Attack successful!");

                if (currentTracker.RecordHit(targetGrid, gridService, selectedTarget, shipType.Value))
                {
                    Console.WriteLine($"You sunk my {shipType}!");
                }
            }
            else
            {
                Console.WriteLine("Sploosh. Attack unsuccessful.");
            }

            if (showContinuePrompt)
            {
                Console.WriteLine("Press any key to continue");
                WaitForKeyPress();
            }

            if (targetGrid.HasShipsRemaining)
            {
                playersTurn = !playersTurn;
                currentPlayer = playersTurn ? "Player" : "Enemy";
                turnNumber++;
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
        return Console.ReadLine() ?? "";
    }

    private static T Request<T>(string request) where T : IParsable<T>
    {
        while (true)
        {
            string input = RequestString(request);
            if (T.TryParse(input, CultureInfo.CurrentCulture, out var result))
            {
                return result;
            }

            Console.WriteLine($"Invalid input '{input}'. Please enter a valid {typeof(T).Name}.");
        }
    }

    private static int RequestInt(string request) => Request<int>(request);

    private static bool RequestBool(string request)
    {
        while (true)
        {
            string input = RequestString(request);
            if (bool.TryParse(input, out bool result))
            {
                return result;
            }

            if (TrueRegex().IsMatch(input))
            {
                return true;
            }

            if (FalseRegex().IsMatch(input))
            {
                return false;
            }

            Console.WriteLine($"Invalid input '{input}'. Please enter a valid {nameof(Boolean)}.");
        }
    }

    private static T RequestEnum<T>(string request) where T : struct, Enum
    {
        while (true)
        {
            string input = RequestString(request);
            if (Enum.TryParse<T>(input, ignoreCase: true, out var result))
            {
                return result;
            }

            Console.WriteLine($"Invalid option '{input}'. Valid options: {string.Join(", ", Enum.GetNames<T>())}");
        }
    }

    private static void DrawGrid(Grid grid, string format)
    {
        string output = grid.ToString(format, CultureInfo.CurrentCulture);
        foreach (char character in output)
        {
            if (GridCharacters.Contains(character))
            {
                Console.ForegroundColor = character switch
                {
                    'h' => ConsoleColor.Red,
                    'm' => ConsoleColor.White,
                    _ => ConsoleColor.Gray
                };

                Console.Write('●');
                Console.ResetColor();
            }
            else
            {
                Console.Write(character);
            }
        }
    }

    private static void DrawPositioningGrid(Grid grid)
    {
        Console.WriteLine();
        DrawGrid(grid, "Positioning");
    }

    private static void DrawTargetingGrid(Grid grid, int roundNumber, string currentPlayer, bool clearConsole, bool showHeader = true)
    {
        if (clearConsole)
        {
            Console.Clear();
            Console.Write("\x1b[3J");
        }
        else
        {
            Console.WriteLine();
        }

        if (clearConsole || showHeader)
        {
            Console.WriteLine($"Round {roundNumber}");
            Console.WriteLine($"{currentPlayer}'s turn.");
        }

        DrawGrid(grid, "Targeting");
    }

    private static void WaitForKeyPress()
    {
        if (Console.IsInputRedirected)
        {
            Console.ReadLine();
        }
        else
        {
            Console.ReadKey(intercept: true);
        }
    }

    [GeneratedRegex(@"^(y(es)?|t(rue)?|1)$", RegexOptions.IgnoreCase)]
    private static partial Regex TrueRegex();

    [GeneratedRegex(@"^(n(o)?|f(alse)?|0)$", RegexOptions.IgnoreCase)]
    private static partial Regex FalseRegex();
}
