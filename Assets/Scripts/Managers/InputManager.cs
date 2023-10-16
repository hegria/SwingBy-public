using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    public Action KeyAction = null;
    public Action<Define.MouseEvent> MouseAction = null;

    bool _pressed = false;
    float _pressedTime = 0.25f;
    float _nowPressed = 0f;

    public void OnUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.anyKey && KeyAction != null)
				KeyAction.Invoke();

        if (MouseAction != null)
        {

            if (Input.GetMouseButton(0) || Input.touchCount > 0)
            {
                _pressed = true;
                _nowPressed += Time.deltaTime;
                if (_nowPressed >= _pressedTime)
                    MouseAction.Invoke(Define.MouseEvent.Press);

            }
            else
            {
                if (_pressed)
                    MouseAction.Invoke(Define.MouseEvent.Click);
                else
                {
                    MouseAction.Invoke(Define.MouseEvent.None);
                }
                _pressed = false;
                _nowPressed = 0f;
            }
        }
    }

    public void Clear()
    {
        KeyAction = null;
        MouseAction = null;
    }
}
