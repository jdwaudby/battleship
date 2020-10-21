using System.Collections.Generic;
using Battleship.Library.Models;
using Battleship.Library.Services.Interfaces;

namespace Battleship.Library.Services.Implementations
{
    public class ShipService : IShipService
    {
        public IEnumerable<Ship> Get()
        {
            return new List<Ship>
            {
                new AircraftCarrier(),
                new Models.Battleship(),
                new Cruiser(),
                new Submarine(),
                new Destroyer()
            };
        }
    }
}