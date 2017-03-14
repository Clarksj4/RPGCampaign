﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalShield : MonoBehaviour
{
    public GameObject[] Prefabs;

    public HexCell Cell;
    public HexDirection Direction;
    public ElementType Element;
    public Character Target;
    public float HoverDistance;
    public Vector3 Scale;

    private GameObject activeModel;

    void Start()
    {
        if (Cell != null)
            Set(Cell, Direction, Element);

        else if (Target != null)
            Set(Target, Direction, Element);
    }

    private void LateUpdate()
    {
        if (Target != null)
        {
            transform.rotation = Quaternion.AngleAxis(30 + (int)Direction * 60, Vector3.up);
            transform.position = Target.transform.position + (transform.forward * HexMetrics.innerRadius) + (Vector3.up * HoverDistance);
        }
    }

    public void Set(Character target, HexDirection direction, ElementType element)
    {
        if (activeModel == null || element != Element)
        {
            if (activeModel != null)
                Destroy(activeModel);

            activeModel = Instantiate(Prefabs[(int)element], transform) as GameObject;
        }

        transform.parent = target.transform;
        transform.localScale = Scale;
        transform.rotation = Quaternion.AngleAxis(30 + (int)direction * 60, Vector3.up);
        transform.position = target.transform.position + (transform.forward * HexMetrics.innerRadius) + (Vector3.up * HoverDistance);
        target.BeginMovement += Target_BeginMovement;
        target.FinishedMovement += Target_FinishedMovement;
    }

    public void Set(HexCell target, HexDirection direction, ElementType element)
    {
        if (activeModel == null || element != Element)
        {
            if (activeModel != null)
                Destroy(activeModel);

            activeModel = Instantiate(Prefabs[(int)element], transform) as GameObject;
        }

        transform.parent = null;
        transform.localScale = Scale;
        transform.rotation = Quaternion.AngleAxis(30 + (int)direction * 60, Vector3.up);
        transform.position = target.Position + (transform.forward * HexMetrics.innerRadius) + (Vector3.up * HoverDistance);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Target_FinishedMovement(object sender, CharacterMovementEventArgs e)
    {
        Show();
    }

    private void Target_BeginMovement(object sender, CharacterMovementEventArgs e)
    {
        Hide();
    }
}