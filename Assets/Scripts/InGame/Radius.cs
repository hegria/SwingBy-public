using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radius : MonoBehaviour
{
    Planet parent_planet; 
    // Start is called before the first frame update
    void Start()
    {
        parent_planet = transform.parent.GetComponent<Planet>();
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        parent_planet.OnTriggerEnter2Dchild(collision);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        parent_planet.OnTriggerStay2Dchild(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        parent_planet.OnTriggerExit2Dchild(collision);
    }
}
