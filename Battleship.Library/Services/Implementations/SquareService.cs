using Battleship.Library.Models;
using Battleship.Library.Services.Interfaces;

namespace Battleship.Library.Services.Implementations
{
    public class SquareService : ISquareService
    {
        public Square Create()
        {
            return new Square();
        }
    }
}
