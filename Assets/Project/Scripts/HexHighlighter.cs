using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pathfinding;

/// <summary>
/// Hexhighlighter with object pooling
/// </summary>
public class HexHighlighter : MonoBehaviour
{
    public Material inRangeMaterial;
    public Material outOfRangeMaterial;
    public GameObject highlightPrefab;
    [Tooltip("The distance between the top of a highlighted cell and the highlight")]
    public float verticalOffset = 0.05f;
    [Tooltip("The size ratio between a hex cell and a highlight")]
    public float sizeRatio = 0.8f;

    // Object pools
    private Queue<GameObject> activeHighlights;
    private Queue<GameObject> inActiveHighlights;

    void Start ()
    {
        activeHighlights = new Queue<GameObject>();
        inActiveHighlights = new Queue<GameObject>();

        CreateHighlights(5);
    }

    /// <summary>
    /// Highlight all cells in the given collection
    /// </summary>
    public void Highlight(ICollection<PathStep> cells)
    {
        // Highlight each cell in collection
        foreach (PathStep step in cells)
            ActivateHighlight(inRangeMaterial, ((HexCell)(step.Node)).Position);
    }

    /// <summary>
    /// Highlight all cells in the path according to whether they are within range of the given distance or not
    /// </summary>
    public void Highlight(Path path, float distance, float timeUnitsToSpend)
    {
        GameObject withinCostHighlight = null;
        float withinCost = 0;

        var walker = path.Steps.First;
        while (walker != null)
        {
            PathStep step = walker.Value;

            if (step.CostTo <= distance)
            {
                withinCostHighlight = ActivateHighlight(inRangeMaterial, ((HexCell)(step.Node)).Position);
                withinCost = step.CostTo;
            }

            else
                ActivateHighlight(outOfRangeMaterial, ((HexCell)(step.Node)).Position);

            walker = walker.Next;
        }

        TextMesh textMesh = withinCostHighlight.GetComponentInChildren<TextMesh>(true);
        textMesh.text = (timeUnitsToSpend - withinCost).ToString();
        textMesh.gameObject.SetActive(true);
    }

    /// <summary>
    /// Stops highlighting all cells
    /// </summary>
    public void Clear()
    {
        int count = activeHighlights.Count;
        for (int i = 0; i < count; i++)
        {
            // Pop
            GameObject highlight = activeHighlights.Dequeue();

            // Turn off
            highlight.transform.GetChild(0).gameObject.SetActive(false);
            highlight.SetActive(false);

            // Move to inactive
            inActiveHighlights.Enqueue(highlight);
        }
    }

    private GameObject ActivateHighlight(Material material, Vector3 position, string text = null)
    {
        if (inActiveHighlights.Count == 0)
            CreateHighlights(1);

        // Pop last item
        GameObject current = inActiveHighlights.Dequeue();

        // Activate
        current.SetActive(true);
        current.GetComponent<Renderer>().sharedMaterial = material;
        current.transform.position = position + (Vector3.up * verticalOffset);
        current.transform.localScale = Vector3.one * (HexMetrics.outerRadius + HexMetrics.innerRadius) * sizeRatio;

        if (text != null)
            current.GetComponentInChildren<TextMesh>().text = text;

        // Save ref
        activeHighlights.Enqueue(current);

        return current;
    }

    /// <summary>
    /// Creates a highlight at the given position offset by verticalOffset, sized by sizeRatio
    /// </summary>
    private void CreateHighlights(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject current = Instantiate(highlightPrefab, transform);
            current.SetActive(false);
            inActiveHighlights.Enqueue(current);
        }
    }

    //private void OnDrawGizmos()
    //{
    //    // Draw destination
    //    if (targetCell != null)
    //        DrawCell(targetCell, Color.white);

    //    // Draw movementPath
    //    if (movementPath != null)
    //    {
    //        foreach (PathStep step in movementPath)
    //        {
    //            // Cell is green if in range
    //            if (step.CostTo <= CurrentCharacter.Stats.CurrentTimeUnits)
    //                DrawCell((HexCell)step.Node, Color.green);

    //            // Cell is red if out of range
    //            else
    //                DrawCell((HexCell)step.Node, Color.red);
    //        }
    //    }

    //    // Draw all cells in range
    //    if (movementRange != null)
    //    {
    //        foreach (PathStep step in movementRange)
    //            DrawCell((HexCell)step.Node, Color.green);
    //    }
    //}

    ///// <summary>
    ///// Draw a cell in the given colour with Gizmo lines
    ///// </summary>
    //private void DrawCell(HexCell cell, Color colour)
    //{
    //    // Set colour, remember old colour
    //    Color oldColour = Gizmos.color;
    //    Gizmos.color = colour;

    //    // Draw line from each vert to next vert
    //    Vector3[] corners = cell.GetCorners();
    //    for (int i = 0; i < corners.Length - 1; i++)
    //        Gizmos.DrawLine(corners[i] + Vector3.up, corners[i + 1] + Vector3.up);
    //    Gizmos.DrawLine(corners.Last() + Vector3.up, corners.First() + Vector3.up);

    //    // Reset colour
    //    Gizmos.color = oldColour;
    //}
}
