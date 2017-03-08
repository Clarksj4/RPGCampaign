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
    [Tooltip("The hex grid this character exists upon")]
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
    private Animator animator;

    /// <summary>
    /// The capacity and current level of each of this characters elements
    /// </summary>
    public Range[] Elements { get; private set; }

    /// <summary>
    /// The element type of this character
    /// </summary>
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
        animator = GetComponentInChildren<Animator>();
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
        Vector3[] pathPoints = path.Select(s => s.Cell.Position).ToArray();

        animator.SetFloat("Speed", 1f);
        float distance = iTween.PathLength(pathPoints);
        float eta = distance / Speed;
        float time = 0;

        float t = 0;
        while (t <= 1f)
        {
            time += Time.deltaTime;
            t = time / eta;

            Vector3 look = iTween.PointOnPath(pathPoints, (time + 4 * Time.smoothDeltaTime) / eta);
            Vector3 lookDir = (look - transform.position).normalized;
            float lookAngle = Vector3.Angle(transform.forward, lookDir);
            float leftOrRight = MathExtension.AngleDir(transform.forward, lookDir.normalized, transform.up);

            animator.SetFloat("Direction", (lookAngle / 20f) * leftOrRight);

            // Face along path, move along path
            transform.LookAt(iTween.PointOnPath(pathPoints, t));
            iTween.PutOnPath(gameObject, pathPoints, t);

            // Update ref to which cell is occupied
            Cell = HexGrid.GetCell(transform.position);

            yield return null;
        }

        animator.SetFloat("Direction", 0);
        animator.SetFloat("Speed", 0);

        if (moving != null)
            StopCoroutine(moving);
        moving = null;

        if (DestinationReached != null)
            DestinationReached(this, new EventArgs());
    }
}
