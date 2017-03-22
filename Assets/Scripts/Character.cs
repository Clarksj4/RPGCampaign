using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[RequireComponent(typeof(Stats))]
public class Character : MonoBehaviour
{
    public event CharacterMovementEventHandler BeginMovement;
    public event CharacterMovementEventHandler FinishedMovement;

    public HexDirection Facing;
    public HexCell Cell;
    [Tooltip("The hex grid this character exists upon")]
    public HexGrid HexGrid;
    [Tooltip("The player that controls this character")]
    public Player Controller;

    private Animator animator;
    private Stats stats;
    private CharacterBehaviour state;

    public Animator Animator { get { return animator; } }
    public Stats Stats { get { return stats; } }
    public bool IsMoving { get { return state.GetType() == typeof(MoveBehaviour); } }


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

        SetState(new IdleBehaviour(this));
    }

    private void Update()
    {
        state.Update();
    }

    public void SetState(CharacterBehaviour newState)
    {
        if (state != null)
        {
            state.Closing();
            if (state is MoveBehaviour && FinishedMovement != null)
                FinishedMovement(this, new CharacterMovementEventArgs(null));
        }

        state = newState;
        newState.Init();

        if (state is MoveBehaviour && BeginMovement != null)
            BeginMovement(this, new CharacterMovementEventArgs(null));
    }

    /// <summary>
    /// Activate this character so it can have its turn. Returns true if this character is able to act.
    /// </summary>
    public bool Activate()
    {
        state.Activate();

        // Return true because nothing can prevent the character from acting
        return true;
    }

    // Push character in given direction the given number of cells
    // Return the number of cells pushed, or -1 if not pushed (due to terrain etc)
    public int Move(HexDirection direction, int distance)
    {


        return -1;
    }

    // Find a path to the given cell and move along it
    public void MoveTo(HexCell cell)
    {
        // Pathfind.QuickestPath()
        // StartCoroutine(DoFollowPath())
    }

    // Find a path to the given cell and move as far along it as possible with current time units
    public void MoveTowards(HexCell cell)
    {

    }

    // Find a path to the closest adjacent cell to the given target and move as far along it as possible with 
    // current time units
    public void MoveTowards(Character target)
    {
        // Pathfind.ClosestPath()
        // StartCoroutine(DoFollowPath())
    }

    public void Move(HexPath path)
    {
        if (path != null && path.Cells > 0)
            SetState(new MoveBehaviour(this, path));

        //if (moving == null && path != null && path.Cells > 0)
        //    moving = StartCoroutine(DoMove(path));
    }

    //IEnumerator DoMove(HexPath path)
    //{
    //    if (BeginMovement != null)
    //        BeginMovement(this, new CharacterMovementEventArgs(path));

    //    Vector3[] pathPoints = path.Points;

    //    float distance = iTween.PathLength(pathPoints);
    //    float eta = distance / stats.Speed;
    //    float time = 0;

    //    float t = 0;
    //    while (t <= 1f)
    //    {
    //        time += Time.deltaTime;
    //        t = time / eta;

    //        // Face along path, move along path
    //        transform.LookAt(iTween.PointOnPath(pathPoints, t));
    //        iTween.PutOnPath(gameObject, pathPoints, t);

    //        // Update the character's animation
    //        UpdateAnimator(pathPoints, time, eta);

    //        // Update reference to the currently occupied cell
    //        UpdateOccupiedCell();

    //        if (ContinuedMovement != null)
    //            ContinuedMovement(this, new CharacterMovementEventArgs(path));

    //        yield return null;
    //    }

    //    if (moving != null)
    //        StopCoroutine(moving);
    //    moving = null;

    //    if (FinishedMovement != null)
    //        FinishedMovement(this, new CharacterMovementEventArgs(path));
    //}

    //private void UpdateOccupiedCell()
    //{
    //    // Update ref to which cell is occupied
    //    Cell.Occupant = null;
    //    Cell = HexGrid.GetCell(transform.position);

    //    // Can't move through an occupied cell
    //    if (Cell.Occupant != null && Cell.Occupant != this)
    //        throw new NotImplementedException("Cannot currently traverse cells that are already occupied");

    //    Cell.Occupant = this;
    //}

    //private void UpdateAnimator(Vector3[] pathPoints, float time, float eta)
    //{
    //    if (animator != null)
    //    {
    //        float t = time / eta;
    //        if (t >= 1f)
    //        {
    //            animator.SetFloat("Speed", 0f);
    //            animator.SetFloat("Direction", 0f);
    //        }

    //        else
    //        {
    //            // Speed
    //            animator.SetFloat("Speed", 1f);

    //            // Focus point for model looking is 4 updates ahead of current position on path
    //            Vector3 focalPoint = iTween.PointOnPath(pathPoints, (time + 4 * Time.smoothDeltaTime) / eta);

    //            // Calculate direction then angle of focal point
    //            Vector3 lookDir = (focalPoint - transform.position).normalized;
    //            float lookAngle = Vector3.Angle(transform.forward, lookDir);

    //            // If the angle to the left or right?
    //            float leftOrRight = MathExtension.AngleDir(transform.forward, lookDir.normalized, transform.up);

    //            animator.SetFloat("Direction", (lookAngle / 20f) * leftOrRight);
    //        }
    //    }
    //}
}
