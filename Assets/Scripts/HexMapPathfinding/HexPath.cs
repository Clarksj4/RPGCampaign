using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HexMapPathfinding
{
    /// <summary>
    /// A path for traversing a hex map. Contains an inorder collection of steps, as well as the cost to traverse them
    /// </summary>
    public class HexPath : IEnumerable<Step>
    {
        /// <summary>
        /// The first step in this path
        /// </summary>
        public HexCell Origin { get { return steps.First().Cell; } }

        /// <summary>
        /// The last step in this path
        /// </summary>
        public HexCell Destination { get { return steps.Last().Cell; } }

        /// <summary>
        /// The total cost of moving along this path
        /// </summary>
        public float Cost { get { return steps.Last().CostTo; } }

        /// <summary>
        /// The number of steps in this path
        /// </summary>
        public int Count { get { return steps.Count; } }

        /// <summary>
        /// The position of each step in this path
        /// </summary>
        public Vector3[] Points
        {
            get
            {
                // Lazy initialisation of point collection
                if (points == null)
                    points = steps.Select(s => s.Cell.Position).ToArray();
                return points;
            }
        }

        private LinkedList<Step> steps;
        private Vector3[] points;

        /// <summary>
        /// An empty path
        /// </summary>
        public HexPath()
        {
            steps = new LinkedList<Step>();
        }

        /// <summary>
        /// Creates a path by walking backwards from the given step
        /// </summary>
        public HexPath(Step final)
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
        public HexPath Truncate(float timeUnits)
        {
            // Iterate through steps adding those that are less that given time units
            HexPath subPath = new HexPath();
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