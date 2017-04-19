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
        }

        public void Insert(TItem item, TPriority priority)
        {
            items.Add(new KeyValuePair<TPriority, TItem>(priority, item));

            int index = items.Count - 1;
            while (index > 0)
            {
                int parentIndex = ParentIndex(index);
                KeyValuePair<TPriority, TItem> parent = items[parentIndex];

                // Percolate
                if (priority.CompareTo(parent.Key) < 0)
                {
                    // Swap item and parent
                    items.Swap(index, parentIndex);
                    index = parentIndex;

                }
            }

        }

        public void DeleteMin()
        {

        }

        public TItem Pop()
        {
            return default(TItem);
        }

        private int ParentIndex(int index)
        {
            return index / 2;
        }

        private int Left(int index)
        {
            return index * 2;
        }

        private int Right(int index)
        {
            return (index * 2) + 1;
        }
    }
}
