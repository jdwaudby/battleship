using Battleship.Library.Models;
using Battleship.Library.Services.Interfaces;

namespace Battleship.Library.Services.Implementations
{
    public class GridService : IGridService
    {
        private readonly ISquareService _squareService;

        public GridService(ISquareService squareService)
        {
            _squareService = squareService;
        }

        public Grid Create(int width, int height)
        {
            var grid = new Grid(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid.Squares[x, y] = _squareService.Create();
                }
            }

            return grid;
        }
    }
}
