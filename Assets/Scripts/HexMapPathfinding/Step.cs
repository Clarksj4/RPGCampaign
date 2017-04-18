
namespace HexMapPathfinding
{
    /// <summary>
    /// A unit of pathfinding for use on a hex map. Contains the cost to move to a given hex as well as the previous step in the path to reach
    /// this step
    /// </summary>
    public class Step
    {
        /// <summary>
        /// The cell being traversed
        /// </summary>
        public HexCell Cell { get; set; }

        /// <summary>
        /// The cell that was previously traversed
        /// </summary>
        public Step Previous { get; set; }

        /// <summary>
        /// The cost to traverse from the beginning of the path up to and including the current cell
        /// </summary>
        public float CostTo { get; set; }

        /// <param name="cell">The cell being traversed</param>
        /// <param name="previous">The cell that was previously traversed</param>
        /// <param name="costTo">The cost to traverse the path from beginning until and including this step</param>
        public Step(HexCell cell, Step previous, float costTo)
        {
            Cell = cell;
            Previous = previous;
            CostTo = costTo;
        }
    }

}