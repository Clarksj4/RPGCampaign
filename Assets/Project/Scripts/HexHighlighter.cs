using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class HexHighlighter : MonoBehaviour
{
    [Tooltip("Prefab for highlighting cells that are in range")]
    public GameObject inRange;
    [Tooltip("Prefab for highlighting cells that are out of range")]
    public GameObject outOfRange;
    [Tooltip("The distance between the top of a highlighted cell and the highlight")]
    public float verticalOffset = 0.05f;
    [Tooltip("The size ratio between a hex cell and a highlight")]
    public float sizeRatio = 0.8f;

    private List<GameObject> highlights;

	void Start ()
    {
        highlights = new List<GameObject>();
	}

    /// <summary>
    /// Highlight all cells in the given collection
    /// </summary>
    public void Highlight(ICollection<PathStep> cells)
    {
        // Highlight each cell in collection
        foreach (PathStep step in cells)
            CreateHighlight(inRange, ((HexCell)(step.Node)).Position);
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
                CreateHighlight(inRange, ((HexCell)(step.Node)).Position);

            // Use out of range prefab
            else
                CreateHighlight(outOfRange, ((HexCell)(step.Node)).Position);
        }
    }

    /// <summary>
    /// Stops highlighting all cells
    /// </summary>
    public void Clear()
    {
        foreach (var highlight in highlights)
            Destroy(highlight);
    }

    /// <summary>
    /// Creates a highlight at the given position offset by verticalOffset, sized by sizeRatio
    /// </summary>
    private void CreateHighlight(GameObject highlight, Vector3 position)
    {
        // Instantiate new prefab as child
        GameObject current = Instantiate(highlight, position + (Vector3.up * verticalOffset), highlight.transform.rotation, transform);

        // Size based on size ratio and cell size
        current.transform.localScale = Vector3.one * (HexMetrics.outerRadius + HexMetrics.innerRadius) * sizeRatio;

        // Remember ref to highlight (so can be deleted later)
        highlights.Add(current);
    }
}
