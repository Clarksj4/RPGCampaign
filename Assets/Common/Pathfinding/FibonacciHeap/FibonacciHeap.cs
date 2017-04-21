using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinding
{
    /// <summary>
    /// Min heap data structure that lazily defers consolidation of trees until delete min operation
    /// </summary>
    public class FibonacciHeap<TPriority, TItem> where TPriority : IComparable<TPriority>
    {
        public LinkedList<HeapNode<TPriority, TItem>> Trees { get; private set; }
        public HeapNode<TPriority, TItem> Minimum { get { return minimumTreesNode.Value; } }

        private LinkedListNode<HeapNode<TPriority, TItem>> minimumTreesNode;

        public FibonacciHeap()
        {
            Trees = new LinkedList<HeapNode<TPriority, TItem>>();
            minimumTreesNode = null;
        }

        public TItem Pop()
        {
            TItem min = Minimum.Value;
            DeleteMin();
            return min;
        }

        public HeapNode<TPriority, TItem> Insert(TPriority priority, TItem value)
        {
            HeapNode<TPriority, TItem> node = new HeapNode<TPriority, TItem>(priority, value);
            Trees.AddFirst(node);

            return node;
        }

        public void DeleteMin()
        {
            if (Minimum == null)
                throw new InvalidOperationException("Heap is empty, cannot remove minimum priority item");

            // Remove minimum element from list of trees
            RemoveRoot(minimumTreesNode);

            // Update new minimum value
            FindMinimum();

            // Consolidate trees
            ConsolidateTrees();
        }

        public void DecreasePriority(HeapNode<TPriority, TItem> node, TPriority priority)
        {
            if (node.Priority.CompareTo(priority) > 0)
                throw new ArgumentException("Priority did not decrease");

            // Decrease key of node
            node.Priority = priority;

            // If heap order violated:
            //  Cut node from parent 
            //  Add to root list of trees
            //  Unmark

            // If parent of node is unmarked:
            //  Mark it

            // Else
            //  Cut parent and meld into root list
            //  Do so recursively for all ancestors

            // Is heap order violated?
            if (node.Parent != null &&
                node.Priority.CompareTo(node.Parent.Priority) < 0)
            {
                HeapNode<TPriority, TItem> parent = MoveToRoot(node);       // Cut from parent and move to root (remember ref to parent)
                node.Marked = false;                                        // Unmark

                // Is new priority less than minimum?
                if (priority.CompareTo(Minimum.Priority) < 0)
                    minimumTreesNode = Trees.First;             // Node is first element in trees list

                // If parent is unmarked, mark it
                if (!parent.Marked)
                    parent.Marked = true;

                else    // Parent IS already marked
                {
                    // Cut parent, move to root, DO FOR ALL ANCESTORS
                    while (parent != null &&
                           parent.Marked)
                    {
                        parent.Marked = false;          // Unmark because its moving to root
                        parent = MoveToRoot(parent);    // Move to root, check its parent
                    }
                }
            }
        }

        private HeapNode<TPriority, TItem> MoveToRoot(HeapNode<TPriority, TItem> node)
        {
            // Remeber ref to parent because child is about to be cut
            HeapNode<TPriority, TItem> parent = node.Parent;

            // Remove ref to parent and parent ref to child
            node.RemoveParent();
            Trees.AddFirst(node);

            // Return old parent of node
            return parent;
        }

        private void RemoveRoot(LinkedListNode<HeapNode<TPriority, TItem>> root)
        {
            // Remove from list of trees
            Trees.Remove(root);

            // Add each child as a root tree, remove ref to parent
            foreach (HeapNode<TPriority, TItem> child in root.Value.Children)
                MoveToRoot(child);
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
            // Bucket index corresponds to the rank of the stored element
            Bucket<LinkedListNode<HeapNode<TPriority, TItem>>> rankBucket = new Bucket<LinkedListNode<HeapNode<TPriority, TItem>>>();

            // Iterate over each root tree
            var walker = Trees.First;
            while (walker != null)
            {
                // Save ref to next root, because walker is going to be overwritten when there is a conflict
                var next = walker.Next;

                // Are there root trees with the same rank?
                int rank = walker.Value.Rank;
                while (rankBucket[rank] != null)
                {
                    
                    walker = Consolidate(walker, rankBucket[rank]);     // Consolidate trees with same rank (also get ref to merged tree)
                    rankBucket[rank] = null;                            // Remove ref to root tree that was consolidated
                    rank = walker.Value.Rank;                           // Update rank
                }

                // No conflicts, store in rank bucket
                rankBucket[rank] = walker;

                walker = next;
            }
        }

        // Merge trees into one tree, maintaining heap property (every child is larger than parent)
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
            root.Value.AddChild(child.Value);
            return root;
        }
    }
}
