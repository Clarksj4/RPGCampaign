using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public float Duration;
    public Button RestartButton;
    public Text Text;

    private Animator animator;
    private float time;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void LateUpdate()
    {
        animator.SetBool("Show", false);
        animator.SetFloat("Time", time);

        if (Duration < 0)
            time = Duration;
        else
            time = Mathf.Clamp(time - Time.deltaTime, -1f, Duration);
    }

    public void CheckGameOver()
    {
        // Game is over if all AI characters are dead
        if (AllCharactersDead(FindObjectOfType<AIPlayer>()))
        {
            Text.text = "Victory!";
            Play();
        }

        // Or all human characters are dead
        else if (AllCharactersDead(FindObjectOfType<HumanPlayer>()))
        {
            Text.text = "Deafeat!";
            Play();
        }
    }

    /// <summary>
    /// Shows the notification animation
    /// </summary>
    public void Play()
    {
        RestartButton.interactable = true;
        RestartButton.GetComponent<Image>().raycastTarget = true;
        time = Duration;

        animator.SetBool("Show", true);
        animator.SetFloat("Time", time);
    }

    /// <summary>
    /// Hides the notification
    /// </summary>
    public void Stop()
    {
        RestartButton.interactable = false;
        RestartButton.GetComponent<Image>().raycastTarget = false;
        time = -1;
    }

    public void Clicked()
    {
        print("Clicked");
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
