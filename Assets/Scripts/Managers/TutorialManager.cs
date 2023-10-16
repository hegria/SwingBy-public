using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager
{

    bool _isFirst;

    public bool isFirst
    {
        get
        {
            _isFirst = PlayerPrefs.GetInt("tutorial") == 0 ? true : false;
            return _isFirst;
        }
        set
        {
            _isFirst = value;
            PlayerPrefs.SetInt("tutorial", _isFirst ? 0 : 1);
        }
    }

    

    public void StartTutorial()
    {
        if (isFirst)
        {
            Managers.UI.ShowSceneUI<UI_TutorialScene>();
            Time.timeScale = 0.001f;
        }
    }
    public void EndTutorial()
    {
        Time.timeScale = 1f;
        isFirst = false;    
        Managers.UI.GetSceneUI<UI_TutorialScene>().Suicide();
    }

}
