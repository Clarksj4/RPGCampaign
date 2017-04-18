using System.Collections.Generic;

namespace Pathfinding
{
    public interface IPathNode
    {
        IEnumerable<IPathNode> Nodes { get; }
    }
}
