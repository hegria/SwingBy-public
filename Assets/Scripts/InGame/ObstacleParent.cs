using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleParent : MonoBehaviour
{
    // Start is called before the first frame update

    public bool isAlreadyBroken = false;

    private void Update()
    {
        if (Managers.Game.WillIDie(transform.position))
            Destroy(gameObject);
    }
}
