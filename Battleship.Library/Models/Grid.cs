namespace Battleship.Library.Models
{
    public class Grid
    {
        public Square[,] Squares { get; protected set; }

        public Grid(int width, int height)
        {
            Squares = new Square[width, height];
        }
    }
}
