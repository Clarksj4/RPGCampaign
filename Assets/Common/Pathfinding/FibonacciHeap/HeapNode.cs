using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinding
{
    public class HeapNode<TPriority, TItem> where TPriority : IComparable<TPriority>
    {
        public HeapNode<TPriority, TItem> Parent { get; set; }
        public List<HeapNode<TPriority, TItem>> Children { get; set; }
        public bool Marked { get; set; }

        public TPriority Priority { get; set; }
        public TItem Value { get; set; }

        public int Rank
        {
            get { return rank; }
            set
            {
                rank = value;
                foreach (var child in Children)
                    child.Rank = value;
            }
        }
        private int rank;

        public HeapNode(TPriority priority, TItem value)
        {
            Priority = priority;
            Value = value;
            rank = -1;
        }
    }
}
