using System;


[Serializable]
public class Attack
{
    public string name = "Basic Attack";
    public int range = 3;
    public float cost = 0;
    public Traverser traverser = Traverser.RangedAttack();
}
