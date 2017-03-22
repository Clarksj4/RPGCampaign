using System;
using UnityEngine;

public class MoveBehaviour : CharacterBehaviour
{
    private HexPath path;
    private float distance;
    private float eta;
    private float time;
    private float t;

    public MoveBehaviour(Character character, HexPath path)
        : base(character)
    {
        this.path = path;

        distance = iTween.PathLength(path.Points);
        eta = distance / character.Stats.Speed;
        time = 0;
        t = 0;
    }

    public override void Update()
    {
        base.Update();

        time += Time.deltaTime;
        t = time / eta;
        if (t <= 1.0f)
        {
            // Face along path, move along path
            character.transform.LookAt(iTween.PointOnPath(path.Points, t));
            iTween.PutOnPath(character.gameObject, path.Points, t);

            // Update the character's animation
            UpdateAnimator();

            // Update reference to the currently occupied cell
            UpdateOccupiedCell();
        }

        SetState(new IdleBehaviour(character));
    }

    private void UpdateOccupiedCell()
    {
        // Update ref to which cell is occupied
        Cell.Occupant = null;
        Cell = HexGrid.GetCell(Transform.position);

        // Can't move through an occupied cell
        if (Cell.Occupant != null && Cell.Occupant != character)
            throw new NotImplementedException("Cannot currently traverse cells that are already occupied");

        Cell.Occupant = character;
    }

    private void UpdateAnimator()
    {
        if (Animator != null)
        {
            float t = time / eta;
            if (t >= 1f)
            {
                Animator.SetFloat("Speed", 0f);
                Animator.SetFloat("Direction", 0f);
            }

            else
            {
                // Speed
                Animator.SetFloat("Speed", 1f);

                // Focus point for model looking is 4 updates ahead of current position on path
                Vector3 focalPoint = iTween.PointOnPath(path.Points, (time + 4 * Time.smoothDeltaTime) / eta);

                // Calculate direction then angle of focal point
                Vector3 lookDir = (focalPoint - Transform.position).normalized;
                float lookAngle = Vector3.Angle(Transform.forward, lookDir);

                // If the angle to the left or right?
                float leftOrRight = MathExtension.AngleDir(Transform.forward, lookDir.normalized, Transform.up);

                Animator.SetFloat("Direction", (lookAngle / 20f) * leftOrRight);
            }
        }
    }
}
