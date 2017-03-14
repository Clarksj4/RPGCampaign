using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class Step
{
    /// <summary>
    /// The cell being traversed
    /// </summary>
    public HexCell Cell;
    /// <summary>
    /// The cell that was previously traversed
    /// </summary>
    public Step Previous;
    /// <summary>
    /// The cost to traverse from the beginning of the path up to and including the current cell
    /// </summary>
    public float CostTo;

    public Step(HexCell cell, Step previous, float costTo)
    {
        Cell = cell;
        Previous = previous;
        CostTo = costTo;
    }
}

