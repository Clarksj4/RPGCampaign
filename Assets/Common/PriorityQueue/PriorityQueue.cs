using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
    public class PriorityQueue<TItem, TPriority> where TPriority : IComparable<TPriority>
    {
        private List<KeyValuePair<TPriority, TItem>> items;

        public PriorityQueue()
        {
            items = new List<KeyValuePair<TPriority, TItem>>();
            items.Add(default(KeyValuePair<TPriority, TItem>));
        }

        public void Insert(TItem item, TPriority priority)
        {
            items.Add(new KeyValuePair<TPriority, TItem>(priority, item));

            int index = items.Count - 1;
            int parentIndex = ParentIndex(index);
            KeyValuePair<TPriority, TItem> parent = items[parentIndex];

            // Percolate upwards
            while (index > 1 && 
                   priority.CompareTo(parent.Key) < 0)
            {
                // Swap item and parent
                items.Swap(index, parentIndex);

                // Update indices
                index = parentIndex;
                parentIndex = ParentIndex(index);
                parent = items[parentIndex];
            }
        }

        public void DeleteMin()
        {
            // Remove root item
            // Move LAST item to root
            // percolate downwards: swap with smallest child until 

            // Remove root item and replace with last item in heap
            items[1] = items[items.Count - 1];

            // Remove last space in heap (item has already been moved)
            items.RemoveAt(items.Count - 1);

            int index = 1;
            KeyValuePair<TPriority, TItem> item = items[index];

            // While not at last position in items
            while (index < items.Count)
            {
                int childIndex = LeftChildIndex(index);

                // If right child is less than left  child
                if (RightChild(index).Key.CompareTo(RightChild(index).Key) < 0)
                    childIndex++;

                // If item is less than child, percolate downwards
                if (item.Key.CompareTo(items[childIndex].Key) < 0)
                {
                    items.Swap(index, childIndex);

                    index = childIndex;
                    item = items[index];
                }

                else break;
            }
        }

        public TItem Pop()
        {
            return default(TItem);
        }

        private int ParentIndex(int childIndex)
        {
            return childIndex / 2;
        }

        private KeyValuePair<TPriority, TItem> Parent(int childIndex)
        {
            return items[ParentIndex(childIndex)];
        }

        private int LeftChildIndex(int parentIndex)
        {
            return parentIndex * 2;
        }

        private KeyValuePair<TPriority, TItem> LeftChild(int parentIndex)
        {
            return items[LeftChildIndex(parentIndex)];
        }

        private int RightChildIndex(int parentIndex)
        {
            return (parentIndex * 2) + 1;
        }

        private KeyValuePair<TPriority, TItem> RightChild(int parentIndex)
        {
            return items[RightChildIndex(parentIndex)];
        }
    }
}
