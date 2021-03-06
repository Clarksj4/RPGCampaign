﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public string WinText = "Victory!";
    public string LoseText = "Defeat!";
    public float Duration;

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
            time = 0;
        else
            time = Mathf.Clamp(time - Time.deltaTime, -1f, Duration);
    }

    public void Victory()
    {
        Play();
    }

    public void Defeat()
    {
        Play();
    }

    /// <summary>
    /// Shows the notification animation
    /// </summary>
    public void Play()
    { 
        time = Duration;

        animator.SetBool("Show", true);
        animator.SetFloat("Time", time);
    }

    /// <summary>
    /// Hides the notification
    /// </summary>
    public void Stop()
    {
        time = -1;
    }

    public void Clicked()
    {
        print("Clicked");
    }
}
