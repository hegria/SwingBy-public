using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Clicked : UI_Base
{

    GameObject image;

    public override void Init()
    {
        image = Util.FindChild(gameObject, "Image");
        StartCoroutine("size_bigger");
    }

    public void SetDir(Vector3 mousePos)
    {

        mousePos.z = 10f;
        Vector3 movedir = new Vector3(Camera.main.ScreenToWorldPoint(mousePos).x - Camera.main.transform.position.x, Camera.main.ScreenToWorldPoint(mousePos).y - Camera.main.transform.position.y, 10);
        transform.localPosition = movedir;
    }
    
    IEnumerator size_bigger()
    {
        for (int i =0; i < 50; i++)
        {
            image.transform.localScale = Vector3.Lerp(new Vector3(0.1f, 0.1f, 1f), new Vector3(0.7f, 0.7f, 1f), i / 50f);
            yield return null;
        }
        Destroy(gameObject);
    }

}
