using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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

    /// <summary>
    /// The character has reached the end of its path
    /// </summary>
    public event EventHandler DestinationReached;

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
        transform.position = Cell.Position;
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

    public void FollowPath(List<Step> path)
    {
        if (moving == null)
            moving = StartCoroutine(DoFollowPath(path));
    }

    IEnumerator DoFollowPath(List<Step> path)
    {
        Vector3[] positionPath = path.Select(s => s.Cell.Position).ToArray();

        float distance = iTween.PathLength(positionPath);
        float eta = distance / Speed;
        float time = 0;

        float t = 0;
        while (t <= 1f)
        {
            time += Time.deltaTime;
            t = time / eta;

            transform.LookAt(iTween.PointOnPath(positionPath, t));
            iTween.PutOnPath(gameObject, positionPath, t);
            Cell = HexGrid.GetCell(transform.position);

            yield return null;
        }

        if (moving != null)
            StopCoroutine(moving);
        moving = null;

        if (DestinationReached != null)
            DestinationReached(this, new EventArgs());
    }
}
