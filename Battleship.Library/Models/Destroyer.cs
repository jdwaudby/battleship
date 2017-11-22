using Battleship.Library.Enums;

namespace Battleship.Library.Models
{
    public class Destroyer : Ship
    {
        public Destroyer()
        {
            Type = ShipType.Destroyer;
            Length = 2;
        }
    }
}
