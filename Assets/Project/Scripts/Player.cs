using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TurnBased;
using TileMap;

public abstract class Player : MonoBehaviour
{
    public Character Current { get; protected set; }
    public List<Character> Allies { get; protected set;}

    protected TurnSystem turnSystem;
    protected ITileMap<Character> grid;

    protected virtual void Awake()
    {
        turnSystem = GetComponentInParent<TurnSystem>();
        grid = FindObjectOfType<HexGrid>();

        // Get list of all characters this player controls
        Allies = GetComponentsInChildren<Character>().ToList();

        //Current = Allies[0];
    }

    public void AddAlly(Character actor)
    {
        Allies.Add(actor);
    }

    public void RemoveAlly(Character actor)
    {
        Allies.Remove(actor);
    }

    public virtual void PawnStart(Character pawn)
    {
        Current = pawn;
    }

    public abstract void PawnDie(Character pawn);
}
