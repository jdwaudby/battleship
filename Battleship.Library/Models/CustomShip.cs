using Battleship.Library.Enums;

namespace Battleship.Library.Models;

public class CustomShip(int length) : Ship(ShipType.Custom, length);