using Battleship.Library.Enums;

namespace Battleship.Library.Models
{
    public class AircraftCarrier : Ship
    {
        public AircraftCarrier()
        {
            Type = ShipType.AircraftCarrier;
            Length = 5;
        }
    }
}
