using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{

    public float Radius { get { return radius;  } }
    

    [SerializeField]
    float radius;
    [SerializeField]
    float minrad = 1.2f;
    [SerializeField]
    float maxrad = 1.5f;
    [SerializeField]
    float gravity = 0.1f;
    [SerializeField]
    bool fixRadius = false;
    

    int spriteidx = 0;

    public int HP
    {
        get { return _hp; }
        set
        {
            _hp = value;
            if (_hp <= 0)
            {
                if (Ship.ship != null)
                    Ship.ship.BrokenObjectByShooting(true);
                Managers.Sound.Play("Boom");
                Managers.Game.GenExplosion("SmallExplosion0.5", transform.position);
                Destroy(gameObject);
            }
        }
    }
    [SerializeField]
    int _hp = 2;


    GameObject orbitObject;

    void Start()
    {
        Init();
    }

    // float 받는 식으로 해야할듯.
    void Init()
    {

        if (!fixRadius)
        {
            while (true)
            {
                radius = Random.Range(minrad, maxrad);
                if (radius + transform.position.x <= Managers.Game.xmax &&
                   -radius + transform.position.x >= -Managers.Game.xmax)
                    break;
            }
        }
        
        // TODO Update에서 변하게 해야함.
        orbitObject = Util.FindChild(gameObject, "Radius");
        orbitObject.transform.parent = null;
        orbitObject.transform.parent = transform;
        float rand = Random.Range(1f, 1.5f);
        gameObject.transform.localScale = new Vector3(rand, rand, 1);
        orbitObject.transform.localScale = new Vector3( Mathf.Pow(1 / rand , 1/3f) * radius * 2f, Mathf.Pow(1 / rand, 1 / 3f) * radius * 2f, 1);

        spriteidx = Random.Range(1, 6);
        

        Sprite spr = Managers.Resource.Load<Sprite>($"Art/Sprite/Planet/행성_{spriteidx}");
        GetComponent<SpriteRenderer>().sprite = spr;
    }

    // Update is called once per frame
    void Update()
    {
        if (Managers.Game.WillIDie(transform.position))
            Destroy(gameObject);
    }

    bool clockwise = false;

    public void OnTriggerEnter2Dchild(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Managers.Game.ComboUpdated(Define.ComboType.SmallSwing);

            Ship.ship.target_planet = gameObject;

            Managers.Game.Fuel += 5f;
            if (Ship.ship.state != Ship.State.OnPath)
            {
                Managers.Game.Fuel += 5f;
            }

            Ship.ship.Speed += 0.5f;
            Ship.ship.SwingGauge += 0.1f;
                
            Managers.Sound.Play("SmallSwing");

            Vector3 tempvec = Vector3.Cross(transform.position - collision.transform.position, collision.transform.up).normalized;
            if (Vector3.Dot(Vector3.forward, tempvec) < 0)
                clockwise = false;
            else
                clockwise = true;
        }
    }
    public void OnTriggerStay2Dchild(Collider2D collision)
    {
        if (collision.tag == "Player" && Ship.ship.state != Ship.State.Targeted)
        {
            //collision.transform.position -=
            //    (collision.transform.position - transform.position).normalized
            //    * gravity * Time.deltaTime;

            Managers.Game.ComboScore += 3f * Time.deltaTime;
            Managers.Game.NowComboTime = 1.0f;

            int dir = 1;
            if (clockwise)
                dir = -1;

            Vector3 goingToward = Vector3.Cross(dir * (transform.position - collision.transform.position), collision.transform.forward).normalized;
            collision.transform.rotation = Quaternion.Lerp(collision.transform.rotation, Quaternion.LookRotation(collision.transform.forward, goingToward), 4* Time.deltaTime);
                // 빨려들어가는거
            
            //collision.transform.rotation = Quaternion.Lerp(
            //    collision.transform.rotation,
            //    Quaternion.AngleAxis(-angle, Vector3.forward), Time.deltaTime
            //    );
        }
    }
    public void OnTriggerExit2Dchild(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Ship.ship.target_planet = null;
        }
    }
}
