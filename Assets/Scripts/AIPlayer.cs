using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player
{
    private Character target;
    private AIBehaviour state;

    private void Start()
    {
        // Find a character that is not one of this player's characters
        target = GameManager.Characters.Where(c => !characters.Contains(c)).Single();
        SetState(new ChaseBehaviour(this));
    }

    private void Update()
    {
        state.Update();
    }

    public void SetState(AIBehaviour newState)
    {
        // If there is a previous state, tell it it is being closed
        if (state != null)
            state.Closing();

        // Save ref to new state, initialize state
        state = newState;
        state.Init();
    }

    public override void Activate(Character actor)
    {
        base.Activate(actor);

        state.Activate();
    }
}
