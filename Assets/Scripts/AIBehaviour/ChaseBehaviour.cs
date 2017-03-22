using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ChaseBehaviour : AIBehaviour
{
    private Character target;

    public ChaseBehaviour(AIPlayer ai, Character target) 
        : base(ai)
    {
        this.target = target;
    }

    public override void ProcessTurn()
    {
        base.ProcessTurn();

        // Get path to target
        // tell character to move
    }
}
