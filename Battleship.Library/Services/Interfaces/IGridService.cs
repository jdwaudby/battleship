using Battleship.Library.Models;

namespace Battleship.Library.Services.Interfaces
{
    public interface IGridService
    {
        Grid Create(int width, int height);
    }
}
