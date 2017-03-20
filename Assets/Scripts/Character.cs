using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[RequireComponent(typeof(Stats))]
public class Character : MonoBehaviour
{
    public event CharacterMovementEventHandler BeginMovement;
    public event CharacterMovementEventHandler ContinuedMovement;
    public event CharacterMovementEventHandler FinishedMovement;

    public HexDirection Facing;
    public HexCell Cell;
    [Tooltip("The hex grid this character exists upon")]
    public HexGrid HexGrid;
    [Tooltip("The player that controls this character")]
    public Player Controller;

    private Coroutine moving;
    private Animator animator;
    private Stats stats;

    public Stats Stats { get { return stats; } }
    public bool IsMoving { get { return moving != null; } }


    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        stats = GetComponent<Stats>();
    }

    private void Start()
    {
        if (HexGrid != null)
        {
            Cell = HexGrid.GetCell(transform.position);
            Cell.Occupant = this;
            transform.position = Cell.Position;
            transform.LookAt(Facing);
        }
    }

    public void MoveTo(HexCell cell)
    {
        // Pathfind.QuickestPath()
        // StartCoroutine(DoFollowPath())
    }

    public void MoveAdjacentTo(Character target)
    {
        // Pathfind.ClosestPath()
        // StartCoroutine(DoFollowPath())
    }

    public void FollowPath(HexPath path)
    {
        if (moving == null && path != null && path.Cells > 0)
            moving = StartCoroutine(DoFollowPath(path));
    }

    IEnumerator DoFollowPath(HexPath path)
    {
        if (BeginMovement != null)
            BeginMovement(this, new CharacterMovementEventArgs(path));

        Vector3[] pathPoints = path.Points;

        if (animator != null)
            animator.SetFloat("Speed", 1f);
        
        float distance = iTween.PathLength(pathPoints);
        float eta = distance / stats.Speed;
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

            if (animator != null)
                animator.SetFloat("Direction", (lookAngle / 20f) * leftOrRight);
            
            // Face along path, move along path
            transform.LookAt(iTween.PointOnPath(pathPoints, t));
            iTween.PutOnPath(gameObject, pathPoints, t);

            // Update ref to which cell is occupied
            Cell.Occupant = null;
            Cell = HexGrid.GetCell(transform.position);

            // Can't move through an occupied cell
            if (Cell.Occupant != null)
                throw new NotImplementedException("Cannot currently traverse cells that are already occupied");

            Cell.Occupant = this;

            if (ContinuedMovement != null)
                ContinuedMovement(this, new CharacterMovementEventArgs(path));

            yield return null;
        }

        if (animator != null)
        {
            animator.SetFloat("Direction", 0);
            animator.SetFloat("Speed", 0);
        }

        if (moving != null)
            StopCoroutine(moving);
        moving = null;

        if (FinishedMovement != null)
            FinishedMovement(this, new CharacterMovementEventArgs(path));
    }
}
