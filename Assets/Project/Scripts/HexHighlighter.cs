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
    public void Highlight(Path path, float distance)
    {
        // Highlight each step in path
        foreach (PathStep step in path)
        {
            // Use in range prefab
            if (step.CostTo <= distance)
                ActivateHighlight(inRangeMaterial, ((HexCell)(step.Node)).Position);

            // Use out of range prefab
            else
                ActivateHighlight(outOfRangeMaterial, ((HexCell)(step.Node)).Position);
        }
    }

    /// <summary>
    /// Stops highlighting all cells
    /// </summary>
    public void Clear()
    {
        for (int i = 0; i < activeHighlights.Count; i++)
        {
            // Pop
            GameObject highlight = activeHighlights.Dequeue();

            // Turn off
            highlight.SetActive(false);

            // Move to inactive
            inActiveHighlights.Enqueue(highlight);
        }
    }

    private void ActivateHighlight(Material material, Vector3 position)
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

        // Save ref
        activeHighlights.Enqueue(current);
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
}
