using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Encapsulates the rules for which cell can be crossed and the cost for doing so
/// </summary>
public interface ITraversable
{
    /// <summary>
    /// Calculates whether a traverser can move in the given direction from the given cell
    /// </summary>
    /// <param name="from">The cell that the traversing object is moving from</param>
    /// <param name="direction">The direction that the traversing object is moving in</param>
    /// <returns>True if the traversing object is able to move in the given direction from the given cell</returns>
    bool IsTraversable(HexCell from, HexDirection direction);

    /// <summary>
    /// Calculates the cost of moving in the given direction from the given cell
    /// </summary>
    /// <param name="from">The cell that the traversing object is moving from</param>
    /// <param name="direction">The direction that the traversing object is moving in</param>
    /// <returns>The cost of moving from the given cell in the given direction</returns>
    float Cost(HexCell from, HexDirection direction);
}
