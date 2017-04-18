﻿
namespace Pathfinding
{
    /// <summary>
    /// A unit of pathfinding. Contains the cost to move to a given node as well as the previous step in the path to reach
    /// this step
    /// </summary>
    public class Step
    {
        /// <summary>
        /// The node being traversed
        /// </summary>
        public IPathNode Node { get; set; }

        /// <summary>
        /// The node that was previously traversed
        /// </summary>
        public Step Previous { get; set; }

        /// <summary>
        /// The cost to traverse from the beginning of the path up to and including the current cell
        /// </summary>
        public float CostTo { get; set; }

        /// <param name="node">The node being traversed</param>
        /// <param name="previous">The node that was previously traversed</param>
        /// <param name="costTo">The cost to traverse the path from beginning until and including this step</param>
        public Step(IPathNode node, Step previous, float costTo)
        {
            Node = node;
            Previous = previous;
            CostTo = costTo;
        }
    }
}