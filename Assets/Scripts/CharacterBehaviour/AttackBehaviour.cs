using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AttackBehaviour : CharacterBehaviour
{
    private Character target;

    public AttackBehaviour(Character character, Character target, Attack attack)
        : base(character)
    {
        this.target = target;
    }

    public override void Init()
    {
        base.Init();

        // TODO: 
        SetState(new IdleBehaviour(character));
    }
}
