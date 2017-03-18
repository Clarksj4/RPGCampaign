using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Character[] Characters { get { return characters; } }
    public HexGrid Grid { get { return grid; } }
    public InitiativeOrder InitiativeOrder { get { return initiativeOrder; } }
    public Character CurrentCharacter { get { return initiativeOrder.Current; } }
    public Player CurrentPlayer { get { return initiativeOrder.Current.Controller; } }

    private Player[] controllers;
    private Character[] characters;
    private HexGrid grid;
    private InitiativeOrder initiativeOrder;


    private void Awake()
    {
        controllers = FindObjectsOfType<Player>();
        characters = FindObjectsOfType<Character>();
        grid = FindObjectOfType<HexGrid>();

        // Create an empty initiative order
        initiativeOrder = new InitiativeOrder();
    }

    private void Start()
    {
        // Add each character to initiative order
        foreach (Character character in characters)
            initiativeOrder.Add(character);

        print(CurrentCharacter + "'s turn, controlled by " + CurrentPlayer);

        // Tell next character to activate!
        CurrentPlayer.Activate(CurrentCharacter);
    }

    public void EndTurn(Character actor)
    {
        initiativeOrder.Cycle();

        print(CurrentCharacter + "'s turn, controlled by " + CurrentPlayer);

        // Tell next character to activate!
        CurrentPlayer.Activate(CurrentCharacter);
    }
}
