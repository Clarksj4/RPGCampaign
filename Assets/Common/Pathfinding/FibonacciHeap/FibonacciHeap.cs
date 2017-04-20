using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinding
{
    public class FibonacciHeap<TPriority, TItem> where TPriority : IComparable<TPriority>
    {
        public LinkedList<HeapNode<TPriority, TItem>> Trees { get; private set; }
        public HeapNode<TPriority, TItem> Minimum { get { return minimumTreesNode.Value; } }
        public List<HeapNode<TPriority, TItem>> Marked { get; private set; }
        public int Count { get; private set; }
        public int MaximumRank { get; private set; }

        private LinkedListNode<HeapNode<TPriority, TItem>> minimumTreesNode;

        public FibonacciHeap()
        {
            Trees = new LinkedList<HeapNode<TPriority, TItem>>();
            minimumTreesNode = null;
            Marked = new List<HeapNode<TPriority, TItem>>();
            Count = 0;
        }

        public HeapNode<TPriority, TItem> Insert(TPriority priority, TItem value)
        {
            HeapNode<TPriority, TItem> node = new HeapNode<TPriority, TItem>(priority, value);
            Trees.AddFirst(node);
            Count++;

            return node;
        }

        public void DeleteMin()
        {
            if (Minimum == null)
                throw new InvalidOperationException("Heap is empty, cannot remove minimum priority item");

            RemoveRoot(minimumTreesNode);

            FindMinimum();

            ConsolidateTrees();
        }

        private void RemoveRoot(LinkedListNode<HeapNode<TPriority, TItem>> root)
        {
            // Remove from list of trees, update item count
            Trees.Remove(root);
            Count--;

            foreach (HeapNode<TPriority, TItem> child in root.Value.Children)
            {
                // Add each child as a root tree, remove ref to parent
                Trees.AddFirst(child);
                child.Parent = null;
                child.Rank -= 1;
            }
        }

        private void FindMinimum()
        {
            // minimum = first tree in list
            // walker through remaining nodes to see if there is a smaller one
            minimumTreesNode = Trees.First;

            // Walk through root element of each root tree
            LinkedListNode<HeapNode<TPriority, TItem>> walker = Trees.First.Next;
            while (walker != null)
            {
                // Check if priority is less than current minimum, update minimum if so
                if (walker.Value.Priority.CompareTo(minimumTreesNode.Value.Priority) < 0)
                    minimumTreesNode = walker;

                // Keep iterating
                walker = walker.Next;
            }
        }

        private void ConsolidateTrees()
        {
            LinkedListNode<HeapNode<TPriority, TItem>>[] rankBucket = new LinkedListNode<HeapNode<TPriority, TItem>>[MaximumRank];

            LinkedListNode<HeapNode<TPriority, TItem>> walker = Trees.First;
            while (walker != null)
            {
                var next = walker.Next;

                // TODO: need to loop incase newly consolidated tree conflicts with another tree
                int rank = walker.Value.Rank;
                if (rankBucket[rank] != null)
                {
                    walker = Consolidate(walker, rankBucket[rank]);
                    rankBucket[rank] = null;
                    rank = walker.Value.Rank;
                    rankBucket[rank] = walker;
                }

                else
                    rankBucket[rank] = walker;

                walker = next;
            }
        }

        private LinkedListNode<HeapNode<TPriority, TItem>> Consolidate(LinkedListNode<HeapNode<TPriority, TItem>> node1, 
                                                                       LinkedListNode<HeapNode<TPriority, TItem>> node2)
        {
            var root = node1;
            var child = node2;

            // is node2 priority less than node2?
            if (node2.Value.Priority.CompareTo(node1.Value.Priority) < 0)
            {
                root = node2;
                child = node1;
            }

            Trees.Remove(child);
            Consolidate(root, child);
            return root;
        }

        // Make other a child of root
        private void Consolidate(HeapNode<TPriority, TItem> root,
                                 HeapNode<TPriority, TItem> other)
        {
            root.Children.Add(other);
            other.Parent = null;

            // Update rank if it's changed
            if (other.Rank >= root.Rank)
                root.Rank = other.Rank + 1;
        }
    }
}
