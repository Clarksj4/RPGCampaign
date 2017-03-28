using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create : MonoBehaviour {

	// Use this for initialization
	void Start () {
       
	}

    // Update is called once per frame
    void Update()
    {

        //0 is left mouse click (Mousebutton down for continuious fire)
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo) && hitInfo.transform.tag == "Enemy")

            {
                //makes clone of what is defined in brackets.
                GameObject projectile = Instantiate(ProjectilePrefab);
                //refernce the projectile to Moving.cs 
               var moving = projectile.GetComponent<Moving>();

                moving.target = hitInfo.transform.gameObject;
            }
        }
    }
    public GameObject ProjectilePrefab;

    
    
}
