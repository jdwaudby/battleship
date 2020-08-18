using System;

namespace Battleship.Library.Exceptions
{
    public class ShipPositioningException : Exception
    {
        public ShipPositioningException()
        {
        }

        public ShipPositioningException(string message)
            : base(message)
        {
        }

        public ShipPositioningException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}