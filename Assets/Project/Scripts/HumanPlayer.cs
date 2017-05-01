using UnityEngine;
using TurnBased;

public class HumanPlayer : Player
{
    public override void PawnStart(ITurnBasedPawn pawn)
    {
        base.PawnStart(pawn);

        Current.FinishedMovement += Actor_FinishedMovement;
    }

    private void Actor_FinishedMovement(object sender, CharacterMovementEventArgs e)
    {
        Current.FinishedMovement -= Actor_FinishedMovement;
        turnSystem.EndTurn();
    }
}
