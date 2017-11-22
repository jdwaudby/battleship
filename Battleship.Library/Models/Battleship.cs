using Battleship.Library.Enums;

namespace Battleship.Library.Models
{
    public class Battleship : Ship
    {
        public Battleship()
        {
            Type = ShipType.Battleship;
            Length = 4;
        }
    }
}
