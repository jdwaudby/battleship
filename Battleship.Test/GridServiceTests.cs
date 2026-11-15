using Battleship.Library.Enums;
using Battleship.Library.Models;
using Battleship.Library.Services.Implementations;
using Xunit;

namespace Battleship.Test;

public class GridServiceTests
{
    [Fact]
    public void Create_ReturnsStandardGrid()
    {
        // Arrange
        var gridService = new GridService();

        // Act
        Grid grid = gridService.Create();

        // Assert
        Assert.Equal(100, grid.Squares.Count);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 1, 1)]
    [InlineData(2, 2, 4)]
    [InlineData(-1, -1, 0)]
    [InlineData(-1, 1, 0)]
    public void Create_ReturnsGrid(int width, int height, int squareCount)
    {
        // Arrange
        var gridService = new GridService();

        // Act
        Grid grid = gridService.Create(width, height);

        // Assert
        Assert.Equal(squareCount, grid.Squares.Count);
        if (squareCount == 0)
        {
            Assert.Equal(string.Empty, grid.ToString());
        }
    }

    [Fact]
    public void Attack_OnShipSquare_AppendsHitFlagAndPreservesShipStatus()
    {
        // Arrange
        var gridService = new GridService();
        Grid grid = gridService.Create();
        var cruiser = new Cruiser();
        gridService.SetShipPosition(grid, cruiser, "B2", Heading.West);

        // Act
        ShipType? shipType = gridService.Attack(grid, "B3");

        // Assert
        Assert.Equal(ShipType.Cruiser, shipType);
        Square square = grid["B3"]!;
        Assert.Equal(SquareStatus.Cruiser | SquareStatus.Hit, square.Status);
        Assert.False(gridService.HasShipBeenSunk(grid, ShipType.Cruiser));
    }

    [Fact]
    public void HasShipBeenSunk_WhenAllSquaresHit_ReturnsTrue()
    {
        // Arrange
        var gridService = new GridService();
        Grid grid = gridService.Create();
        var destroyer = new Destroyer();
        gridService.SetShipPosition(grid, destroyer, "A1", Heading.West);

        // Act
        gridService.Attack(grid, "A1");
        bool sunkAfterFirstHit = gridService.HasShipBeenSunk(grid, ShipType.Destroyer);
        gridService.Attack(grid, "A2");
        bool sunkAfterSecondHit = gridService.HasShipBeenSunk(grid, ShipType.Destroyer);

        // Assert
        Assert.False(sunkAfterFirstHit);
        Assert.True(sunkAfterSecondHit);
    }

    [Fact]
    public void GetValidNeighbouringTargets_AfterSingleHit_ReturnsOrthogonalValidNeighbours()
    {
        // Arrange
        var gridService = new GridService();
        Grid grid = gridService.Create();

        // Act
        var neighbours = gridService.GetValidNeighbouringTargets(grid, "B3").ToList();

        // Assert
        Assert.Equal(4, neighbours.Count);
        Assert.Contains("C3", neighbours);
        Assert.Contains("A3", neighbours);
        Assert.Contains("B2", neighbours);
        Assert.Contains("B4", neighbours);
    }

    [Fact]
    public void GetValidNeighbouringTargets_WithAxisConstraint_ReturnsOnlyTargetsOnAxis()
    {
        // Arrange
        var gridService = new GridService();
        Grid grid = gridService.Create();
        var cruiser = new Cruiser();
        gridService.SetShipPosition(grid, cruiser, "B2", Heading.West);

        gridService.Attack(grid, "B3");
        gridService.Attack(grid, "B4");

        // Act
        var horizontalNeighbours = gridService.GetValidNeighbouringTargets(grid, "B4", y: "B").ToList();

        // Assert
        Assert.Single(horizontalNeighbours);
        Assert.Equal("B5", horizontalNeighbours[0]);
    }

    [Fact]
    public void UnsunkHitPositions_OrdersByLastUpdatedAndEnablesBacktracking()
    {
        // Arrange
        var gridService = new GridService();
        Grid grid = gridService.Create();
        var cruiser = new Cruiser();
        gridService.SetShipPosition(grid, cruiser, "B2", Heading.West); // B2, B3, B4

        // Hit middle (B3), then right (B4), then miss (B5)
        gridService.Attack(grid, "B3");
        Thread.Sleep(5);
        gridService.Attack(grid, "B4");
        Thread.Sleep(5);
        gridService.Attack(grid, "B5");

        // Act
        var deadEndTargets = gridService.GetValidNeighbouringTargets(grid, "B4", y: "B").ToList();
        string? firstHitTarget = grid.UnsunkHitPositions.FirstOrDefault();
        var backtrackTargets = gridService.GetValidNeighbouringTargets(grid, firstHitTarget!, y: "B").ToList();

        // Assert
        Assert.Empty(deadEndTargets);
        Assert.Equal("B3", firstHitTarget);
        Assert.Single(backtrackTargets);
        Assert.Equal("B2", backtrackTargets[0]);
    }

    [Fact]
    public void ParityTargets_OnStandardGrid_ReturnsHalfOfSquares()
    {
        // Arrange
        var gridService = new GridService();
        Grid grid = gridService.Create();

        // Act
        var parityTargets = grid.ParityTargets.ToList();

        // Assert
        Assert.Equal(50, parityTargets.Count);
        Assert.Contains("A2", parityTargets);
        Assert.DoesNotContain("A1", parityTargets);
    }

    [Fact]
    public void TargetTracker_LocksAxisAndBacktracksToSinkShip()
    {
        // Arrange
        var gridService = new GridService();
        Grid grid = gridService.Create();
        var cruiser = new Cruiser();
        gridService.SetShipPosition(grid, cruiser, "B2", Heading.West); // B2, B3, B4
        var tracker = new TargetTracker();

        // Act & Assert: 1. Initial hit on B3
        ShipType? hit1 = gridService.Attack(grid, "B3");
        Assert.False(tracker.RecordHit(grid, gridService, "B3", hit1!.Value));
        Assert.Equal("B3", tracker.LastTarget);

        // 2. Second hit on B4 locks onto row B
        Thread.Sleep(5);
        ShipType? hit2 = gridService.Attack(grid, "B4");
        Assert.False(tracker.RecordHit(grid, gridService, "B4", hit2!.Value));
        Assert.Equal("B", tracker.NextYTarget);
        Assert.Equal("", tracker.NextXTarget);

        // 3. Next target from B4 along row B must be B5 (miss)
        string nextTarget = tracker.SelectTarget(grid, gridService);
        Assert.Equal("B5", nextTarget);
        Thread.Sleep(5);
        Assert.Null(gridService.Attack(grid, nextTarget));

        // 4. Next target must backtrack to B3 and select B2
        string backtrackTarget = tracker.SelectTarget(grid, gridService);
        Assert.Equal("B2", backtrackTarget);
        ShipType? hit3 = gridService.Attack(grid, backtrackTarget);
        Assert.True(tracker.RecordHit(grid, gridService, backtrackTarget, hit3!.Value));
        Assert.Equal("", tracker.LastTarget);
    }
}
