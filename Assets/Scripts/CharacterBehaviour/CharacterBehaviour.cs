using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

public class CharacterBehaviour
{
    public Animator Animator { get { return character.Animator; } }
    public HexGrid HexGrid { get { return character.HexGrid; } }
    public Transform Transform { get { return character.transform; } }
    public HexCell Cell
    {
        get { return character.Cell; }
        set { character.Cell = value; }
    }

    protected Character character;

    public CharacterBehaviour(Character character)
    {
        this.character = character;
    }

    public virtual void Init() { }
    public virtual void Update() { }
    public virtual void Activate() { } 
    public virtual void Closing() { }

    protected void SetState(CharacterBehaviour newState)
    {
        character.SetState(newState);
    }

    protected void StartCoroutine(IEnumerator routine)
    {
        character.StartCoroutine(routine);
    }
}
