using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_MainScene : UI_Scene
{
    
    enum Buttons
    {
        LeaderBoard,
        Setting,
        Start,
        Credit,
        Tutorial,
        Apply
    }


    enum GameObjects
    {
        SettingPopup,
        CreditPopup,
        EntireObjects,
    }

    enum Scrollbars
    {
        Sound,
        Music
    }

    float combodeltax;
    float combodeltay;

    List<UI_ComboText> combolist = new List<UI_ComboText>();

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Scrollbar>(typeof(Scrollbars));
        Get<Button>((int)Buttons.Start).gameObject.BindEvent(StartGame);
        Get<Button>((int)Buttons.LeaderBoard).gameObject.BindEvent(ShowLeaderBoard);
        Get<Button>((int)Buttons.Credit).gameObject.BindEvent(ShowCredit);
        Get<Button>((int)Buttons.Setting).gameObject.BindEvent(Setting);
        Get<Button>((int)Buttons.Tutorial).gameObject.BindEvent(ShowTutorial);
        Get<Button>((int)Buttons.Apply).gameObject.BindEvent(ShowApply);
        GetObject((int)GameObjects.SettingPopup).BindEvent(ShowApply);
        Get<Scrollbar>((int)Scrollbars.Sound).onValueChanged.AddListener((float val) => OnSoundChanged(false, val));
        Get<Scrollbar>((int)Scrollbars.Music).onValueChanged.AddListener((float val) => OnSoundChanged(true, val));
        GetObject((int)GameObjects.CreditPopup).BindEvent(CloseCredit);
        GetObject((int)GameObjects.EntireObjects).GetComponent<RectTransform>().offsetMax = new Vector2(0, -Util.getSafeTopArea() -Managers.Ads.getHeight());

        GetObject((int)GameObjects.SettingPopup).SetActive(false);
        GetObject((int)GameObjects.CreditPopup).SetActive(false);
    }
            
    public void StartGame(PointerEventData eventData)
    {
        Managers.Sound.Play("UIClick");
        ((GameScene)(Managers.Scene.CurrentScene)).StartGame();
    }

    public void ShowLeaderBoard(PointerEventData eventData)
    {
        Managers.Sound.Play("UIClick");

        Managers.GPGS.ShowLeaderBoard();
    }

    public void ShowCredit(PointerEventData eventData)
    {
        Managers.Sound.Play("UIClick");

        GetObject((int)GameObjects.CreditPopup).SetActive(true);
    }
    public void CloseCredit(PointerEventData eventData)
    {
        Managers.Sound.Play("UIClick");

        GetObject((int)GameObjects.CreditPopup).SetActive(false);
    }

    public void Setting(PointerEventData eventData)
    {
        Managers.Sound.Play("UIClick");

        Get<Scrollbar>((int)Scrollbars.Sound).value = Managers.Sound.EffectVolumn;
        Get<Scrollbar>((int)Scrollbars.Music).value = Managers.Sound.BGMVolumn;
        GetObject((int)GameObjects.SettingPopup).SetActive(true);

    }

    void OnSoundChanged(bool isBGM, float val)
    {
        if (isBGM)
        {
            Managers.Sound.BGMVolumn = val;
        }
        else
        {
            Managers.Sound.EffectVolumn = val;
        }

    }

    public void ShowTutorial(PointerEventData eventData)
    {
        Managers.Sound.Play("UIClick");

        SystemLanguage sl = Application.systemLanguage;

        string msg;

        if (!Managers.Tutorial.isFirst)
        {
            Managers.Tutorial.isFirst = true;

            

            if (sl != SystemLanguage.Korean)
            {
                msg = "Tutorial is enabled.";
            }
            else
            {
                msg = "튜토리얼이 활성화 되었습니다.";
            }
            ShowToastMessage(msg, 1.0f);
        } 
        else
        {
            if (sl != SystemLanguage.Korean)
            {
                msg = "Tutorial is already enabled!";
            }
            else
            {
                msg = "튜토리얼이 이미 활성화 되어있습니다!";
            }
            ShowToastMessage(msg, 1.0f);
        }

    }

    AndroidJavaObject toastClass;

    public void ShowToastMessage(string message, float time)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }

        Invoke("Cloasetoast", time);
    }

    public void Cloasetoast()
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                if (toastClass != null) toastClass.Call("cancel");
            }));
        }
    }

    public void ShowApply(PointerEventData eventData)
    {
        Managers.Sound.Play("UIClick");

        GetObject((int)GameObjects.SettingPopup).SetActive(false);

    }


}
