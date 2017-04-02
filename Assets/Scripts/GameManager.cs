using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string SpellPath;

    public Character[] Characters { get { return characters; } }
    public HexGrid Grid { get { return grid; } }
    public InitiativeOrder InitiativeOrder { get { return initiativeOrder; } }
    public Character CurrentCharacter { get { return initiativeOrder.Current; } }
    public Player CurrentPlayer { get { return initiativeOrder.Current.Controller; } }
    public SpellBook SpellBook { get { return spellBook; } }

    private Player[] controllers;
    private Character[] characters;
    private HexGrid grid;
    private InitiativeOrder initiativeOrder;
    private SpellBook spellBook;

    private void Awake()
    {
        controllers = FindObjectsOfType<Player>();
        characters = FindObjectsOfType<Character>();
        grid = FindObjectOfType<HexGrid>();
        spellBook = new SpellBook("Spells");

        // Create an empty initiative order
        initiativeOrder = new InitiativeOrder();
    }

    private void Start()
    {
        // Add each character to initiative order
        foreach (Character character in characters)
            initiativeOrder.Add(character);

        //print(CurrentCharacter + "'s turn, controlled by " + CurrentPlayer);

        // Tell next character to activate!
        CurrentCharacter.Activate();
        CurrentPlayer.Activate(CurrentCharacter);
    }

    public void EndTurn(Character actor)
    {
        initiativeOrder.Cycle();

        //print(CurrentCharacter + "'s turn, controlled by " + CurrentPlayer);

        // Tell next character to activate!
        CurrentCharacter.Activate();
        CurrentPlayer.Activate(CurrentCharacter);
    }
}
