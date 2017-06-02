using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public UnityEvent Victory;
    public UnityEvent Defeat;

    public TurnSystem turnSystem;

    void Awake()
    {
        turnSystem = FindObjectOfType<TurnSystem>();
    }

    public void CheckGameOver()
    {
        // Game is over if all AI characters are dead
        if (AllCharactersDead(FindObjectOfType<AIPlayer>()))
        {
            turnSystem.Paused = true;
            Victory.Invoke();
        }
            
        // Or all human characters are dead
        else if (AllCharactersDead(FindObjectOfType<HumanPlayer>()))
        {
            turnSystem.Paused = true;
            Defeat.Invoke();
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene("AutoLoadHexMap");
    }

    bool AllCharactersDead(Player controller)
    {
        bool charactersAllDead = true;

        Character[] characters = FindObjectsOfType<Character>();
        foreach (Character character in characters)
        {
            if (character.Controller == controller &&
                character.Stats.CurrentHP > 0)
            {
                charactersAllDead = false;
                break;
            }
        }

        return charactersAllDead;
    }
}
