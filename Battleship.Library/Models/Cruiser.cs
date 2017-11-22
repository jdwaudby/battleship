using Battleship.Library.Enums;

namespace Battleship.Library.Models
{
    public class Cruiser : Ship
    {
        public Cruiser()
        {
            Type = ShipType.Cruiser;
            Length = 3;
        }
    }
}
