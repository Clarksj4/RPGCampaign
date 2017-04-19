using System.Collections.Generic;

namespace Pathfinding
{
    /// <summary>
    /// Interface for pathfinding node
    /// </summary>
    public interface IPathNode
    {
        /// <summary>
        /// Nodes accessible from this one
        /// </summary>
        IEnumerable<IPathNode> Nodes { get; }
    }
}
