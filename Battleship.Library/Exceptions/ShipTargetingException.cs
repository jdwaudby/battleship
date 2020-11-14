using System;

namespace Battleship.Library.Exceptions
{
    public class ShipTargetingException : Exception
    {
        public ShipTargetingException()
        {
        }

        public ShipTargetingException(string message)
            : base(message)
        {
        }

        public ShipTargetingException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}