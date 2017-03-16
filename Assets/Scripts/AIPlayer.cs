using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player
{
    IEnumerator DoEndTurnAfterDelay(float time, Character actor)
    {
        yield return new WaitForSeconds(time);
        EndTurn(actor);
    }

    public override void Activate(Character actor)
    {
        StartCoroutine(DoEndTurnAfterDelay(2, actor));
    }

    public override void EndTurn(Character actor)
    {
        TurnSystem.Cycle();
    }
}
