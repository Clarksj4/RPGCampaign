using UnityEngine;
using System;

public enum ElementType { Air, Earth, Fire, Water }

[ExecuteInEditMode]
public class GameMetrics : MonoBehaviour
{
    public static GameMetrics Instance { get; private set; }

    public Element this[ElementType index] { get { return Elements[(int)index]; } }

    public float ElementCapacity = 100f;

    [HideInInspector]
    public Element[] Elements;

    private void Awake()
    {
        if (Instance != null)
            throw new ArgumentException("Only one GameMetrics allowed");
        Instance = this;

        if (!Application.isEditor)
            DontDestroyOnLoad(this);
    }

    private void Reset()
    {
        Elements = new Element[]
        {
            new Air(),
            new Earth(),
            new Fire(),
            new Water()
        };
    }
}
