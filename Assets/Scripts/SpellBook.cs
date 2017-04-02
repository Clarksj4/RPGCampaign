using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpellBook
{
    private Dictionary<string, Spell> catalogue;

    public Spell this[string key] { get { return catalogue[key]; } }

    public SpellBook()
    {
        catalogue = new Dictionary<string, Spell>();
    }

    public SpellBook(string path) 
        : this()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Spells");
        foreach (GameObject prefab in prefabs)
        {
            Spell spell = prefab.GetComponent<Spell>();
            Add(spell);
        }
    }

    public void Add(Spell spell)
    {
        catalogue.Add(spell.name, spell);
    }
}
