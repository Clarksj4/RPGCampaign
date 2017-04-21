using System.Collections.Generic;

namespace Pathfinding
{
    /// <summary>
    /// Dynamically sized bucket
    /// </summary>
    public class Bucket<TItem>
    {
        public TItem this[int index]
        {
            get
            {
                if (!bucket.ContainsKey(index))
                    bucket.Add(index, default(TItem));

                return bucket[index];
            }

            set
            {
                if (!bucket.ContainsKey(index))
                    bucket.Add(index, value);
                else
                    bucket[index] = value;
            }
        }

        private Dictionary<int, TItem> bucket;

        public Bucket()
        {
            bucket = new Dictionary<int, TItem>();
        }
    }
}
