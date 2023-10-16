using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comet : MonoBehaviour
{
    [SerializeField]
    float speed = 6f;
    [SerializeField]
    Vector3 movdir;
    float upperto;

    // Start is called before the first frame update
    void Start()
    {
        speed = 6f;
        movdir += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
        if ( movdir.x >= 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, +90 + Mathf.Rad2Deg * Mathf.Atan2(movdir.y,movdir.x));
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, +270 + Mathf.Rad2Deg * Mathf.Atan2(movdir.y, movdir.x));
        }
        upperto = 7.5f;
    }


    // Update is called once per frame
    void Update()
    {
        if (Ship.ship == null)
            return;

        if (Ship.ship.transform.position.y + upperto >= transform.position.y)
            transform.position += movdir.normalized * Time.deltaTime * speed;

        if (Managers.Game.WillIDie(transform.position))
            Destroy(gameObject);
    }
}
