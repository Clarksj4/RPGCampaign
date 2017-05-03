using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CastBehaviour : CharacterBehaviour
{
    private HexCell target;

    public CastBehaviour(Character character, HexCell target) 
        : base(character)
    {
        character.LookAt(target);

        Animator.SetTrigger("Cast");
    }
}
