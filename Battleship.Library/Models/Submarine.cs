using Battleship.Library.Enums;

namespace Battleship.Library.Models
{
    public class Submarine : Ship
    {
        public Submarine()
        {
            Type = ShipType.Submarine;
            Length = 3;
        }
    }
}
