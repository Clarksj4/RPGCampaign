using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Event handler delegate for when an element meter has changed
/// </summary>
/// <param name="sender">The object that raised the event.</param>
/// <param name="e">Event arguments containing the element type of the meter that was changed</param>
public delegate void ElementMeterEventHandler(object sender, ElementMeterEventArgs e);

/// <summary>
/// Event arguments for when a element meter has changed
/// </summary>
public class ElementMeterEventArgs
{
    /// <summary>
    /// The element type of the meter that was changed
    /// </summary>
    public ElementType Type { get; set; }

    public ElementMeterEventArgs(ElementType type)
    {
        Type = type;
    }
}
