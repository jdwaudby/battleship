﻿using Battleship.Library.Enums;

namespace Battleship.Library.Models
{
    public class Square
    {
        public Square(string y, int x)
        {
            Y = y;
            X = x.ToString();
        }
        
        public string Y { get; }
        public string X { get; }
        public string Coordinates => Y + X;
        public SquareStatus? Status { get; set; }
    }
}
