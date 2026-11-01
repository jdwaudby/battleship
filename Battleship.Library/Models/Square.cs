using Battleship.Library.Enums;

namespace Battleship.Library.Models;

public class Square(string y, int x)
{
    public string Y { get; } = y;
    public string X { get; } = x.ToString();
    public string Coordinates => $"{Y}{X}";
    public SquareStatus? Status { get; private set; }
    public DateTime LastUpdated { get; private set; } = DateTime.UtcNow;

    public void UpdateStatus(SquareStatus status)
    {
        if (Status == status)
        {
            return;
        }

        if (Status is null)
        {
            Status = status;
        }
        else
        {
            Status |= status;
        }

        LastUpdated = DateTime.UtcNow;
    }
}
