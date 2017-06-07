using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

public class CharacterBehaviour
{
    protected Character character;

    public CharacterBehaviour(Character character)
    {
        this.character = character;
    }

    public virtual void Init() { }
    public virtual void Closing() { }

    public virtual void Update() { }

    protected void SetState(CharacterBehaviour newState)
    {
        character.SetState(newState);
    }

    protected void StartCoroutine(IEnumerator routine)
    {
        character.StartCoroutine(routine);
    }
}
