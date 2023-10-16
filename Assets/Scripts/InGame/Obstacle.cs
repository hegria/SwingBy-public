using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    [SerializeField]
    bool isrotate = false;
    [SerializeField]
    float _nowAngleSpeed = 20f;
    [SerializeField]
    bool ismove = false;
    [SerializeField]
    Vector3 moveTowordVector;
    [SerializeField]
    float speed = 2.2f;
    [SerializeField]
    float glitchraius = 0.5f;
    float upperto;
    [SerializeField]
    public int order;


    bool hasparent = false;
    bool isGlitched = false;
    bool isHitted = false;

    Vector3 originaldir;
    GameObject parent;
    private void Start()
    {
        originaldir = transform.position;
        parent = transform.parent.gameObject;
        if (parent.GetComponent<ObstacleParent>())
        {
            hasparent = true;
        }
        upperto = 8f;
    }

    public void SetHitted()
    {
        isHitted = true;
    }
    
    public void SetMovofBigObstacle()
    {
        moveTowordVector.x = Random.Range(-0.3f, 0.3f);
        moveTowordVector.Normalize();
        moveTowordVector = 0.9f * moveTowordVector;
    }

    private void Update()
    {
        if (!Managers.Game.isGameStart)
            return;

        if (!Ship.ship)
            return;


        if (Ship.ship.transform.position.y + upperto >= transform.position.y)
        {

            if (isrotate)
            {
                transform.RotateAround(parent.transform.position, transform.forward, _nowAngleSpeed * Time.deltaTime);
            }
            if (ismove)
            {
                transform.Translate(moveTowordVector.normalized * speed * Time.deltaTime);
            }

        }

        bool glitchcheck = true;

        if(hasparent)
        {
            if (parent.GetComponent<ObstacleParent>().isAlreadyBroken)
            {
                glitchcheck = false;
            }
        }

        if (!isHitted && glitchcheck && !isGlitched)
        {
            
            if ( (Ship.ship.transform.position-transform.position).magnitude <= glitchraius
                && Mathf.Abs(Vector3.Dot((Ship.ship.transform.position - transform.position).normalized, Ship.ship.transform.up)) <= 0.25f)
            {
                isGlitched = true;
                Ship.ship.SwingGauge += 0.05f;
                Managers.Game.Fuel += 3f;
                Managers.Game.ComboUpdated(Define.ComboType.Graze);
            }
        }

        if (Managers.Game.WillIDie(transform.position))
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GetComponent<BigObstacle>())
            return;

        if (collision.tag == "Fuel" || collision.tag == "Planet")
        {
            if (collision.tag == "Planet" && order >=2)
            {
                Destroy(gameObject);
            }
            else
            {
                Managers.Sound.Play("SmallBoom");
                Managers.Game.GenExplosion("SmallExplosion0.25", transform.position);
                Destroy(gameObject);
            }
        } else if (collision.tag == "Obstacle")
        {
            if (collision.gameObject.GetComponent<Obstacle>().order >= order)
            {

                Managers.Sound.Play("SmallBoom", volumn:0.5f);
                Managers.Game.GenExplosion("SmallExplosion0.25", transform.position);
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
