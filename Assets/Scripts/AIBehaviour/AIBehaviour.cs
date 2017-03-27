using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIBehaviour
{
    protected AIPlayer ai;

    public GameManager GameManager { get { return ai.GameManager; } }
    public Character Current { get { return ai.Current; } }
    public List<Character> Characters { get { return ai.Characters; } }

    public AIBehaviour(AIPlayer ai)
    {
        this.ai = ai;
    }

    public virtual void Init() { }
    public virtual void Update() { }
    public virtual void Activate() { }
    public virtual void Closing() { }

    public virtual void EndTurn()
    {
        ai.EndTurn();
    }
}
