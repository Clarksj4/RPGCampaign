using System;
using System.Collections.Generic;

namespace HexMapPathfinding
{
    public static class HexDirectionExtension
    {
        /// <summary>
        /// Enumerable of all the different hex directions
        /// </summary>
        public static IEnumerable<HexDirection> All()
        {
            // Get all directions
            Array directionValues = Enum.GetValues(typeof(HexDirection));

            // Iterate through collection returning each one
            for (int i = 0; i < directionValues.Length; i++)
                yield return (HexDirection)directionValues.GetValue(i);
        }
    }
}