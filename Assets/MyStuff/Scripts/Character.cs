using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character : MonoBehaviour
{
    /// <summary>
    /// The current value of one of this character's elements has changed
    /// </summary>
    public event ElementMeterEventHandler ElementValueChanged;

    /// <summary>
    /// The capacity of one of this character's elements has changed
    /// </summary>
    public event ElementMeterEventHandler ElementCapacityChanged;

        public HexCell Cell;
    public HexGrid HexGrid;

    [Tooltip("The amount of time units this character has available each turn")]
    public Range TimeUnits;

    [Tooltip("The speed at which this character moves")]
    public float Speed;

    [Tooltip("Which cells can be crossed by this character and the cost of doing so")]
    public Traverser Traverser;

    [Tooltip("The element that this character is spec'd in. Determines the capacity this character has for each of the elements, as well " +
        "as which elements are strong against this character.")]
    [SerializeField]
    private ElementType element;
    private new Renderer renderer;
    private Coroutine moving;

    /// <summary>
    /// The capacity and current level of each of this characters elements
    /// </summary>
    public Range[] Elements { get; private set; }

    public ElementType Element
    {
        get { return element; }
        set
        {
            element = value;
            UpdateModelColour();
        }
    }

    private void Awake()
    {
        renderer = GetComponentInChildren<Renderer>();

        Elements = new Range[4];
        for (int i = 0; i < Elements.Length; i++)
            Elements[i] = new Range();
    }

    private void Start()
    {
        Cell = HexGrid.GetCell(transform.position);
    }

    private void OnValidate()
    {
        UpdateModelColour();
    }

    /// <summary>
    /// Get this character's capacity for the given element
    /// </summary>
    public float GetElementCapacity(ElementType type)
    {
        return Elements[(int)type].Max;
    }

    /// <summary>
    /// Get this character's current level of the given element
    /// </summary>
    public float GetElementValue(ElementType type)
    {
        return Elements[(int)type].Current;
    }

    /// <summary>
    /// Set this character's capacity for the given element type
    /// </summary>
    public void SetElementCapacity(ElementType type, float capacity)
    {
        Elements[(int)type].Max = capacity;

        if (ElementCapacityChanged != null)
            ElementCapacityChanged(this, new ElementMeterEventArgs(type));
    }

    /// <summary>
    /// Set this character's current level of the given element type
    /// </summary>
    public void SetElementValue(ElementType type, float value)
    {
        Elements[(int)type].Current = value;

        if (ElementValueChanged != null)
            ElementValueChanged(this, new ElementMeterEventArgs(type));
    }

    /// <summary>
    /// Update the character model to reflect its element type
    /// </summary>
    private void UpdateModelColour()
    {
        // null checks for when executed in edit mode
        if (renderer != null && GameMetrics.Instance != null)
            renderer.material.color = GameMetrics.Instance.Elements[(int)Element].Colour;
    }

    public void MoveTo(Vector3 position)
    {
        if (moving == null)
            moving = StartCoroutine(DoMoveTo(position));
    }

    public void FollowPath(List<Step> path)
    {
        if (moving == null)
        {
            moving = StartCoroutine(DoFollowPath(path));
            //moving = StartCoroutine(DoFollowCurve(path[0].Cell.Position, path[1].Cell.Position, path[2].Cell.Position));
        }
            
    }

    IEnumerator DoFollowPath(List<Step> path)
    {
        int i = 0;
        if (path.Count - i >= 3)
        {
            HexCell start = path[i++].Cell;
            HexCell mid = path[i++].Cell;
            HexCell end = path[i++].Cell;

            // While travelling in a straight line
            while (i < path.Count &&
                    start.GetDirection(mid) == mid.GetDirection(end))
            {
                start = mid;
                mid = end;
                end = path[i].Cell;

                i++;
            }

            yield return StartCoroutine(DoMoveTo(start.Position));
            yield return StartCoroutine(DoFollowCurve(start.Position, mid.Position, end.Position));
        }
            
        // For remainder of cells in path, just move to them
        for (; i < path.Count; i++)
            yield return StartCoroutine(DoMoveTo(path[i].Cell.Position));

        if (moving != null)
            StopCoroutine(moving);
        moving = null;
    }

    IEnumerator DoMoveTo(Vector3 position)
    {
        transform.LookAt(position);
        while (transform.position != position)
        {
            transform.position = Vector3.MoveTowards(transform.position, position, Speed * Time.deltaTime);
            yield return null;
        }
        
        // Update occupied cell when reaching a new cell
        Cell = HexGrid.GetCell(transform.position);
    }

    IEnumerator DoFollowCurve(Vector3 start, Vector3 m, Vector3 end)
    {
        // Approximation of the distance of curve
        float distance = Vector3.Distance(start, m) + Vector3.Distance(m, end);
        float eta = distance / Speed;
        float time = 0;
        while (transform.position != end)
        {
            time += Time.deltaTime;
            float t = time / eta;

            Vector3 point = Bezier.GetPoint(start, m, end, t);
            transform.LookAt(point);
            transform.position = point;

            // Update occupied cell when reaching a new cell
            Cell = HexGrid.GetCell(transform.position);

            yield return null;
        }
    }
}
