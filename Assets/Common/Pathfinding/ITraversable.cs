﻿
namespace Pathfinding
{
    /// <summary>
    /// Interface for pathfinding node traversal rules and costs
    /// </summary>
    public interface ITraversable
    {
        /// <summary>
        /// Can a path travel between the two adjacent nodes?
        /// </summary>
        /// <param name="begin">The beginning node</param>
        /// <param name="adjacent">The destination node</param>
        /// <returns>True if traversal is allowed from the beginning node to the adjacent node</returns>
        bool IsTraversable(IPathNode begin, IPathNode adjacent);

        /// <summary>
        /// What costs is associated with travelling between the two adjacent nodes?
        /// </summary>
        /// <param name="begin">The beginning node</param>
        /// <param name="adjacent">The destination node</param>
        /// <returns>The cost associated with travelling from the beginning node to the adjacent 
        /// node</returns>
        float Cost(IPathNode begin, IPathNode adjacent);
    }
}