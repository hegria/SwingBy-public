using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigObstacle: MonoBehaviour
{
    public int HP
    {
        get { return _hp; }
        set
        {
            _hp = value;
            if (_hp <=0 )
            {
                if (Ship.ship != null)
                    Ship.ship.BrokenObjectByShooting();
                Managers.Sound.Play("Boom");
                Managers.Game.GenExplosion("BigExplosion", transform.position);

                Destroy(gameObject);
            } 
        }
    }
    [SerializeField]
    int _hp = 3;

    // Update is called once per frame
    void Update()
    {
        
    }
}
