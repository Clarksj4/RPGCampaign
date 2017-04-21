using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinding
{
    public class HeapNode<TPriority, TItem> where TPriority : IComparable<TPriority>
    {
        public HeapNode<TPriority, TItem> Parent { get; set; }
        public HashSet<HeapNode<TPriority, TItem>> Children { get; set; }
        public bool Marked { get; set; }
        public int Rank { get { return Children.Count; } }

        public TPriority Priority { get; set; }
        public TItem Value { get; set; }

        public HeapNode(TPriority priority, TItem value)
        {
            Children = new HashSet<HeapNode<TPriority, TItem>>();

            Priority = priority;
            Value = value;
        }

        public void AddChild(HeapNode<TPriority, TItem> node)
        {
            Children.Add(node);
            node.Parent = this;
        }

        public void RemoveChild(HeapNode<TPriority, TItem> child)
        {
            Children.Remove(child);
            child.Parent = null;
        }

        public void RemoveParent()
        {
            Parent.Children.Remove(this);
            Parent = null;
        }
    }
}
