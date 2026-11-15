using System.Text.RegularExpressions;
using Battleship.Library.Enums;
using Battleship.Library.Services.Interfaces;

namespace Battleship.Library.Models;

public partial class TargetTracker
{
    public string LastTarget { get; private set; } = "";
    public string NextYTarget { get; private set; } = "";
    public string NextXTarget { get; private set; } = "";

    public string SelectTarget(Grid targetGrid, IGridService gridService)
    {
        if (string.IsNullOrEmpty(LastTarget))
        {
            LastTarget = targetGrid.UnsunkHitPositions.FirstOrDefault() ?? "";
        }

        List<string>? validTargets = null;

        if (!string.IsNullOrEmpty(LastTarget))
        {
            validTargets = [.. gridService.GetValidNeighbouringTargets(targetGrid, LastTarget, NextYTarget, NextXTarget)];

            if (validTargets.Count == 0)
            {
                string? firstHitTarget = targetGrid.UnsunkHitPositions.FirstOrDefault();
                if (firstHitTarget is not null)
                {
                    LastTarget = firstHitTarget;
                    validTargets = [.. gridService.GetValidNeighbouringTargets(targetGrid, LastTarget, NextYTarget, NextXTarget)];

                    if (validTargets.Count == 0)
                    {
                        NextYTarget = NextXTarget = "";

                        foreach (string hitPosition in targetGrid.UnsunkHitPositions)
                        {
                            var neighbours = gridService.GetValidNeighbouringTargets(targetGrid, hitPosition).ToList();
                            if (neighbours.Count > 0)
                            {
                                LastTarget = hitPosition;
                                validTargets = neighbours;
                                break;
                            }
                        }
                    }
                }

                if (validTargets is null || validTargets.Count == 0)
                {
                    LastTarget = NextYTarget = NextXTarget = "";
                }
            }
        }

        if (validTargets is null || validTargets.Count == 0)
        {
            if (targetGrid.HasStandardShipsRemaining)
            {
                validTargets = [.. targetGrid.ParityTargets];
            }

            if (validTargets is null || validTargets.Count == 0)
            {
                validTargets = [.. targetGrid.ValidTargets];
            }
        }

        return validTargets[Random.Shared.Next(validTargets.Count)];
    }

    public bool RecordHit(Grid targetGrid, IGridService gridService, string selectedTarget, ShipType shipType)
    {
        if (gridService.HasShipBeenSunk(targetGrid, shipType))
        {
            NextYTarget = NextXTarget = "";
            LastTarget = targetGrid.UnsunkHitPositions.FirstOrDefault() ?? "";
            return true;
        }

        var (selectedX, selectedY) = SplitCoordinates(selectedTarget);

        if (!string.IsNullOrEmpty(LastTarget))
        {
            var (lastX, lastY) = SplitCoordinates(LastTarget);

            if (lastX == selectedX)
            {
                NextXTarget = selectedX;
            }

            if (lastY == selectedY)
            {
                NextYTarget = selectedY;
            }
        }

        LastTarget = selectedTarget;
        return false;
    }

    internal static (string x, string y) SplitCoordinates(string coords)
    {
        var match = CoordinatesRegex().Match(coords);
        return (match.Groups[2].Value, match.Groups[1].Value);
    }

    [GeneratedRegex(@"^([A-Za-z]+)(\d+)$")]
    private static partial Regex CoordinatesRegex();
}
