using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    public event EventHandler AttackBegun;
    public event EventHandler AttackApex;
    public event EventHandler AttackComplete;
    public event EventHandler CastKneadingBegun;
    public event EventHandler CastKneadingComplete;
    public event EventHandler CastApex;
    public event EventHandler CastComplete;
    public event EventHandler IdleBegun;

    private void TestMethod()
    {
        print("Test method called!");
    }

    public void NotifyIdle()
    {
        if (IdleBegun != null)
            IdleBegun(this, new EventArgs());
    }

    public void NotifyAttackBegun()
    {
        if (AttackBegun != null)
            AttackBegun(this, new EventArgs());
    }

    public void NotifyAttackApex()
    {
        if (AttackApex != null)
            AttackApex(this, new EventArgs());
    }

    public void NotifyAttackComplete()
    {
        if (AttackComplete != null)
            AttackComplete(this, new EventArgs());
    }

    public void NotifyCastKneadingBegun()
    {
        if (CastKneadingBegun != null)
            CastKneadingBegun(this, new EventArgs());
    }

    public void NotifyCastKneadingComplete()
    {
        if (CastKneadingComplete != null)
            CastKneadingComplete(this, new EventArgs());
    }

    public void NotifyCastApex()
    {
        if (CastApex != null)
            CastApex(this, new EventArgs());
    }

    public void NotifyCastComplete()
    {
        if (CastComplete != null)
            CastComplete(this, new EventArgs());
    }
}
