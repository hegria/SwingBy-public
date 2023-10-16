using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update

    Vector3 movdir = new Vector3(0,1,0);
    float bulletSpd = 20.0f;
    private void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            return;
        }

        bool isBig = false;
        if (collision.gameObject.tag == "Obstacle" || collision.gameObject.tag == "Planet" || collision.gameObject.tag == "Comet")
        {
            if (Ship.ship == null)
                return;
            

            if (collision.gameObject.GetComponent<BigObstacle>())
            {
                isBig = true;
                collision.gameObject.GetComponent<BigObstacle>().HP -= 1;

            }
            else if (collision.gameObject.GetComponent<Planet>())
            {
                isBig = true;
                collision.gameObject.GetComponent<Planet>().HP -= 1;
            }
            else
            {
                Ship.ship.BrokenObjectByShooting();
                Destroy(collision.gameObject);
            }
        }

        if(collision.gameObject.tag != "Untagged")
        {

            Managers.Sound.Play("Hit");
            if (!isBig)
                Managers.Sound.Play("SmallBoom");
            
            Managers.Game.GenExplosion("BulletBoom", transform.position);
            Destroy(gameObject);

        }
    }


    // Update is called once per frame
    void Update()
    {
        transform.Translate(movdir * Time.deltaTime * bulletSpd);
        if (Managers.Game.WillIDie(transform.position))
            Destroy(gameObject);

    }
}
