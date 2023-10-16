using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_GameButton : MonoBehaviour
{

    public bool NowPressing
    {
        get { return _nowpressing; }
        set
        {
            _nowpressing = value;
            if (_nowpressing)
                OnPressDown();
            else
                OnPressUp();
        }
    } 

    float presstime = 0;
    bool _pressed = false;
    bool _nowpressing = false;
    float clicktime = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.BindEvent(FirePress, Define.UIEvent.Down);
        gameObject.BindEvent(FireUp, Define.UIEvent.Up);
    }

    // Update is called once per frame
    void Update()
    {
        if (_pressed)
            presstime += Time.deltaTime;
        

        if (!NowPressing &&presstime >= clicktime)
            NowPressing = true;

        if (NowPressing)
            OnPressing();

    }
    public void FirePress(PointerEventData eventData)
    {
        _pressed = true;
        presstime = 0;
    }

    public void FireUp(PointerEventData eventData)
    {
        _pressed = false;
        if (presstime <= clicktime)
        {
            Onclick();
        }
        else 
        {
            presstime = 0;
            NowPressing = false;

        }
    }

    protected virtual void Onclick() { }

    protected virtual void OnPressDown() { }
    protected virtual void OnPressUp() { }

    protected virtual void OnPressing() { }

}
