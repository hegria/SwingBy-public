using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_ComboText : UI_Base
{
    
    private void Start()
    {
        Init();
    }

    public override void Init()
    {

    }

    public void SetText(string str)
    {
        gameObject.GetComponent<TextMeshProUGUI>().SetText(str);
    }
}
