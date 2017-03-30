using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpellBook : MonoBehaviour
{
    public Spell[] Spells;

    public Spell Get(string name)
    {
        return Spells.Where(s => s.name == name).Single();
    }
}
