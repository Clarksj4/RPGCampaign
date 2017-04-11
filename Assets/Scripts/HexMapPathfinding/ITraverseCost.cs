public interface ITraverseCost
{
    /// <summary>
    /// Calculates the cost of moving in the given direction from the given cell
    /// </summary>
    /// <param name="from">The cell that the traversing object is moving from</param>
    /// <param name="direction">The direction that the traversing object is moving in</param>
    /// <returns>The cost of moving from the given cell in the given direction</returns>
    float Cost(HexCell origin, HexDirection direction);
}
