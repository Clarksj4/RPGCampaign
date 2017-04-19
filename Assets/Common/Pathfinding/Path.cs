using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Pathfinding
{
    /// <summary>
    /// A path for traversing a hex map. Contains an inorder collection of steps, as well as the cost to traverse them
    /// </summary>
    public class Path : IEnumerable<Step>
    {
        /// <summary>
        /// The first step in this path
        /// </summary>
        public IPathNode Origin { get { return steps.First().Node; } }

        /// <summary>
        /// The last step in this path
        /// </summary>
        public IPathNode Destination { get { return steps.Last().Node; } }

        /// <summary>
        /// The total cost of moving along this path
        /// </summary>
        public float Cost { get { return steps.Last().CostTo; } }

        /// <summary>
        /// The number of steps in this path
        /// </summary>
        public int Count { get { return steps.Count; } }

        private LinkedList<Step> steps;

        /// <summary>
        /// An empty path
        /// </summary>
        public Path()
        {
            steps = new LinkedList<Step>();
        }

        /// <summary>
        /// Creates a path by walking backwards from the given step
        /// </summary>
        public Path(Step final)
            : this()
        {
            // Iterate through given cell's chain of cells
            Step walker = final;
            while (walker != null)
            {
                // Add to cell as first cell in path
                steps.AddFirst(walker);
                walker = walker.Previous;
            }
        }

        /// <summary>
        /// Adds a step to the path
        /// </summary>
        public void AddStep(Step step)
        {
            steps.AddLast(step);
        }

        /// <summary>
        /// Returns a new path beginning at origin and containing all steps in this path less than the 
        /// given time units
        /// </summary>
        public Path Truncate(float timeUnits)
        {
            // Iterate through steps adding those that are less that given time units
            Path subPath = new Path();
            foreach (Step step in this)
            {
                if (step.CostTo <= timeUnits)
                    subPath.AddStep(step);

                // Stop once steps are out of time units range
                else
                    break;
            }

            // All steps that are within cost
            return subPath;
        }

        public IEnumerator<Step> GetEnumerator()
        {
            return steps.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return steps.GetEnumerator();
        }
    }
}