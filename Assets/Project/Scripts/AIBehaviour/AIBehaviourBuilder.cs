//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using FluentBehaviourTree;

//public class AIBehaviourBuilder
//{
//    private Func<Character> selectTarget;
//    private Func<List<Ability>> prioritizeAbilities;
//    private AIPlayer ai;

//    public AIBehaviourBuilder(AIPlayer ai)
//    {
//        this.ai = ai;
//    }

//    public AIBehaviourBuilder TargetSelection(Func<Character> selectTarget)
//    {
//        this.selectTarget = selectTarget;
//        return this;
//    }

//    public AIBehaviourBuilder AbilityPriority(Func<List<Ability>> prioritizeAbilities)
//    {
//        this.prioritizeAbilities = prioritizeAbilities;
//        return this;
//    }

//    public IBehaviourTreeNode Build()
//    {

//    }
//}
