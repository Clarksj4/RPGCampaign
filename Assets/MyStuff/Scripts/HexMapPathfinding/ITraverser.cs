public interface ITraverser
{
    bool IsTraversable(HexCell cell, HexDirection direction);
    float TraverseCost(HexCell cell, HexDirection direction);
}
