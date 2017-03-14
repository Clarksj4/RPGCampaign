using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalShield : MonoBehaviour
{
    public GameObject[] Prefabs;

    public HexCell Cell;
    public HexDirection Direction;
    public ElementType Element;
    public Transform Target;
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
            transform.position = Target.position + (transform.forward * HexMetrics.innerRadius) + (Vector3.up * HoverDistance);
        }
    }

    public void Set(Transform target, HexDirection direction, ElementType element)
    {
        if (activeModel == null || element != Element)
        {
            if (activeModel != null)
                Destroy(activeModel);

            activeModel = Instantiate(Prefabs[(int)element], transform) as GameObject;
        }

        transform.parent = target;
        transform.localScale = Scale;
        transform.rotation = Quaternion.AngleAxis(30 + (int)direction * 60, Vector3.up);
        transform.position = target.position + (transform.forward * HexMetrics.innerRadius) + (transform.up * HoverDistance);
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
        transform.position = target.Position + (transform.forward * HexMetrics.innerRadius) + (transform.up * HoverDistance);
    }
}
