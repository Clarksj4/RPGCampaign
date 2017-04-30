using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class HexHighlighter : MonoBehaviour
{
    public GameObject inRange;
    public GameObject outOfRange;
    public float verticalOffset = 0.05f;
    public float sizeRatio = 0.8f;

    private List<GameObject> highlights;

	// Use this for initialization
	void Start ()
    {
        highlights = new List<GameObject>();
	}

    public void Highlight(ICollection<PathStep> cells)
    {
        foreach (PathStep step in cells)
            CreateHighlight(inRange, ((HexCell)(step.Node)).Position);
    }

    public void Highlight(Path path, float distance)
    {
        foreach (PathStep step in path)
        {
            if (step.CostTo <= distance)
                CreateHighlight(inRange, ((HexCell)(step.Node)).Position);

            else
                CreateHighlight(outOfRange, ((HexCell)(step.Node)).Position);
        }
    }

    public void Clear()
    {
        foreach (var highlight in highlights)
            Destroy(highlight);
    }

    private void CreateHighlight(GameObject highlight, Vector3 position)
    {
        GameObject current = Instantiate(highlight, position + (Vector3.up * verticalOffset), highlight.transform.rotation, transform);
        current.transform.localScale = Vector3.one * (HexMetrics.outerRadius + HexMetrics.innerRadius) * sizeRatio;
        highlights.Add(current);
    }
}
