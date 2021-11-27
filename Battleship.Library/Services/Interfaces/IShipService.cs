using Battleship.Library.Models;

namespace Battleship.Library.Services.Interfaces
{
    public interface IShipService
    {
        IEnumerable<Ship> Get();
    }
}