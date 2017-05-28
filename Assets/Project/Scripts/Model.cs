using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    [Tooltip("The speed at which this character moves")]
    public float WalkSpeed;
    public float RunSpeed;

    [Header("Body Locations")]
    public Transform Head;
    public Transform Torso;
    public Transform LeftHand;
    public Transform RightHand;
    public Transform CastSpawn;

    private Animator animator;
    private Dictionary<string, Action> eventActions = new Dictionary<string, Action>();

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Walk(bool active)
    {
        animator.SetBool("Moving", active);
    }

    public void Run(bool active)
    {
        animator.SetBool("Running", active);
    }

    public void Hurt(Action hurtComplete = null)
    {
        AddEventAction("HurtComplete", hurtComplete);

        animator.SetTrigger("Hurt");
    }

    public void MeleeAttack(Action attackApex, Action attackComplete)
    {
        AddEventAction("AttackApex", attackApex);
        AddEventAction("AttackComplete", attackComplete);

        animator.SetTrigger("Attack");
    }

    public void KneadingCast(Action kneadingBegun, Action kneadingComplete, Action castApex, Action castComplete)
    {
        AddEventAction("KneadingBegun", kneadingBegun);
        AddEventAction("KneadingComplete", kneadingComplete);
        AddEventAction("CastApex", castApex);
        AddEventAction("CastComplete", castComplete);

        animator.SetTrigger("Cast");
    }

    public void Die(Action deathComplete)
    {
        AddEventAction("DeathComplete", deathComplete);

        animator.SetBool("Dead", true);
    }

    public void Victory(Action victoryComplete)
    {
        AddEventAction("VictoryComplete", victoryComplete);

        animator.SetTrigger("Victory");
    }

    public void Defeat(Action defeatComplete)
    {
        AddEventAction("DefeatComplete", defeatComplete);

        animator.SetTrigger("Defeat");
    }
    
    private void AddEventAction(string key, Action action)
    {
        eventActions.Remove(key);
        if (action != null)
            eventActions.Add(key, action);
    }

    private void NotifyAnimationEvent(string eventName)
    {
        eventActions[eventName]();
        eventActions.Remove(eventName);
    }
}
