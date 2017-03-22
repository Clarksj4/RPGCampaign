using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIBehaviour
{
    protected AIPlayer ai;

    public AIBehaviour(AIPlayer ai)
    {
        this.ai = ai;
    }

    public virtual void Init() { }
    public virtual void Update() { }
    public virtual void ProcessTurn() { }
    public virtual void Closing() { }
}
