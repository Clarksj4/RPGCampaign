using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player
{
    public CharacterInput InputSystem;

    public override void Activate(Character actor)
    {
        InputSystem.Selected = actor;
        actor.FinishedMovement += Actor_FinishedMovement;
    }

    public override void EndTurn(Character actor)
    {
        TurnSystem.Cycle();
        actor.FinishedMovement -= Actor_FinishedMovement;
    }

    private void Actor_FinishedMovement(object sender, CharacterMovementEventArgs e)
    {
        EndTurn(sender as Character);
    }
}
