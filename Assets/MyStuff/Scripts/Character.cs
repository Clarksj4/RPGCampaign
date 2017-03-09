using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[RequireComponent(typeof(Stats))]
public class Character : MonoBehaviour
{
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

    private Coroutine moving;
    private Animator animator;
    private Stats stats;

    public Stats Stats { get { return stats; } }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        stats = GetComponent<Stats>();
    }

    private void Start()
    {
        Cell = HexGrid.GetCell(transform.position);
        transform.position = Cell.Position;
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
