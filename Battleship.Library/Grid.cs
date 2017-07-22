namespace Battleship.Library
{
    public class Grid
    {
        public Square[,] Squares { get; protected set; }

        public Grid(int width, int height)
        {
            Squares = new Square[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Squares[x,y] = new Square();
                }
            }
        }
    }
}
