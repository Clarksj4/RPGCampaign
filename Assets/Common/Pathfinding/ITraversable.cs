
namespace Pathfinding
{
    /// <summary>
    /// Encapsulates the rules for which nodes can be crossed and the cost for doing so
    /// </summary>
    public interface ITraversable
    {
        bool IsTraversable(IPathNode from, IPathNode adjacent);

        float Cost(IPathNode from, IPathNode adjacent);
    }
}