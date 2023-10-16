using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class onDebug : MonoBehaviour
{

    TextMeshProUGUI mytext;
    // Start is called before the first frame update
    void Start()
    {
        mytext = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Ship.ship != null)
        {
            mytext.SetText($"{Ship.ship.Speed:F2}");

        }
    }
}
