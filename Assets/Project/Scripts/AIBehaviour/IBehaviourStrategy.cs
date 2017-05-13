using System;
using System.Collections.Generic;
using System.Linq;
using FluentBehaviourTree;

public interface IBehaviourStrategy
{
    BehaviourTreeStatus Update();
    void PawnStart(Character current);
}
