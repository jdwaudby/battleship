using Battleship.Library.Enums;

namespace Battleship.Library.Models
{
    public class CustomShip : Ship
    {
        public CustomShip(int length)
        {
            Type = ShipType.Custom;
            Length = length;
        }
    }
}