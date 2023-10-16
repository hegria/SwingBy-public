using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UI_GameScene : UI_Scene
{
    enum TMPs
    {
        Distance,
        HighHeight,
        HighScore,
        NowHigh,
        NowScore,
        NowCombo,
        Score,
        CurrentCombo
    }
    enum GameObjects
    {
        EntireObjects,

        GameOver,
        Playing,
        ComboBar,
        Combos,
        SettingPopup,
    }
    enum Buttons
    {
        Retry,
        Left,
        Flight,
        Right,
        Pause,
        Resume,
        Replay,
        Home,
        ReHome,
    }
    enum Images
    {
        SwingGauge1,
        SwingGauge2,
        SpeedGauge1,
        FuelGauge1,
        ComboGauge
    }

    enum Scrollbars
    {
        Sound,
        Music
    }

    float combodeltax;
    float combodeltay;

    bool isPaused;

    List<UI_ComboText> combolist = new List<UI_ComboText>();

    const int gaugewidth = 320;

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(TMPs));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
        Bind<Scrollbar>(typeof(Scrollbars));

        GetButton((int)Buttons.Retry).gameObject.BindEvent(RestartGame);
        GetButton((int)Buttons.Pause).gameObject.BindEvent(PauseGame);
        GetButton((int)Buttons.Home).gameObject.BindEvent(GotoMain);


        GetButton((int)Buttons.Replay).gameObject.BindEvent(RestartGame);
        GetButton((int)Buttons.Resume).gameObject.BindEvent(ResumeGame);
        GetButton((int)Buttons.ReHome).gameObject.BindEvent(GotoMain);

        Get<Scrollbar>((int)Scrollbars.Sound).onValueChanged.AddListener((float val) => OnSoundChanged(false, val));
        Get<Scrollbar>((int)Scrollbars.Music).onValueChanged.AddListener((float val) => OnSoundChanged(true, val));
        GetObject((int)GameObjects.EntireObjects).GetComponent<RectTransform>().offsetMax = new Vector2(0, -Util.getSafeTopArea() - Managers.Ads.getHeight());

    }

    public void SudoInit()
    {

        GetImage((int)Images.ComboGauge).fillAmount = 0f;
        GetObject((int)GameObjects.GameOver).SetActive(false);
        GetObject((int)GameObjects.SettingPopup).SetActive(false);


        SetObjectsActive(false);
        combodeltax = GetObject((int)GameObjects.ComboBar).GetComponent<RectTransform>().anchoredPosition.x;
        combodeltay = GetObject((int)GameObjects.ComboBar).GetComponent<RectTransform>().anchoredPosition.y;
        GetObject((int)GameObjects.ComboBar).SetActive(false);

        Managers.Game.onCombo -= ComboUIUpdate;
        Managers.Game.onCombo += ComboUIUpdate;

        

        combolist = new List<UI_ComboText>();

    }

    float nowupdatetime = 0.1f ;
    const float nextupdatetime = 0.1f;

    private void Update()
    {
        if (Ship.ship == null)
        {
            return;
        }

        nowupdatetime += Time.deltaTime;
        if ( nowupdatetime >= nextupdatetime)
        {
            GetTMP((int)TMPs.Distance).SetText($"{(int)Managers.Game.CurrentHeight}KM");
            nowupdatetime = 0;
        }

        GetTMP((int)TMPs.Score).SetText($"{Managers.Game.CurrentComboscore}");
        GetImage((int)Images.ComboGauge).rectTransform.offsetMax = new Vector2(- gaugewidth * (1 - ( Managers.Game.NowComboTime / GameManager.ComboTime)), 0);

        if (Managers.Game.NowComboCount > 0)
        {
            GetTMP((int)TMPs.CurrentCombo).SetText($"{(int)Managers.Game.ComboScore} ¡¿ {Managers.Game.NowComboCount}");
        }
        
        if (Ship.ship.SwingGauge >1.0f)
        {
            GetImage((int)Images.SwingGauge1).fillAmount = 1.0f;
            GetImage((int)Images.SwingGauge2).fillAmount = Ship.ship.SwingGauge - 1.0f;
        }
        else
        {
            GetImage((int)Images.SwingGauge1).fillAmount = Ship.ship.SwingGauge;
            GetImage((int)Images.SwingGauge2).fillAmount = 0f;
        }

        GetImage((int)Images.FuelGauge1).rectTransform.offsetMax = new Vector2(-gaugewidth * (1 - (Managers.Game.Fuel / Managers.Game.MaxFuel)), 0);
        GetImage((int)Images.SpeedGauge1).rectTransform.offsetMax = new Vector2(-gaugewidth * (1 - ((Ship.ship.Speed - 2f) / (Ship.ship.MaxSpeed  - 2f) )), 0);
    }

    public void SetObjectsActive(bool LiveorDie)
    {
        GetObject((int)GameObjects.EntireObjects).SetActive(LiveorDie);
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
    public void SetGameOver()
    {
        GetTMP((int)TMPs.HighHeight).SetText($"{(int)Managers.Game.HighHeight}KM");
        GetTMP((int)TMPs.HighScore).SetText($"{(int)Managers.Game.HighScore}");
        GetTMP((int)TMPs.NowHigh).SetText($"{(int)Managers.Game.CurrentHeight}KM");
        GetTMP((int)TMPs.NowScore).SetText($"{(int)Managers.Game.NowScore}");
        GetTMP((int)TMPs.NowCombo).SetText($"{(int)Managers.Game.CurrentComboscore}");
        GetObject((int)GameObjects.GameOver).SetActive(true);
        GetObject((int)GameObjects.Playing).SetActive(false);
    }

    public void ComboUIUpdate(Define.ComboType comboType)
    {

        if (comboType == Define.ComboType.ComboEnd)
        {
            int nowcont = combolist.Count;
            for (int i = 0; i < nowcont; i++)
            {
                UI_ComboText combo = combolist[0];
                combolist.RemoveAt(0);
                Destroy(combo.gameObject);
            }
            GetObject((int)GameObjects.ComboBar).SetActive(false);
            return;
        }

        GameObject obj = GetObject((int)GameObjects.Combos);

        UI_ComboText combotext = Managers.UI.MakeSubItem<UI_ComboText>(obj.transform);
        combotext.GetComponent<RectTransform>().localScale = Vector3.one;
        combolist.Add(combotext);

        if (Managers.Game.NowComboCount == 1)
        {
            GetObject((int)GameObjects.ComboBar).SetActive(true);
            combotext.SetText(comboType.ToString());
        }
        else
        {
            combotext.SetText("+ " + comboType.ToString());
        }

        if (Managers.Game.NowComboCount >= 10)
        {
            UI_ComboText combo = combolist[0];
            combolist.RemoveAt(0);
            Destroy(combo.gameObject);
            int nowcont = combolist.Count;
            for (int i = 0; i< nowcont; i++)
            {
                UI_ComboText com = combolist[i];
                com.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -60 * i);
            }
        }
        else
        {
            int nowcomboCount = Managers.Game.NowComboCount > 10 ? 10 : Managers.Game.NowComboCount;
            combotext.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -60 * (nowcomboCount - 1));
            GetObject((int)GameObjects.ComboBar).GetComponent<RectTransform>().anchoredPosition
                = new Vector2(combodeltax, combodeltay - 60 * (nowcomboCount - 1)) ;
        }
    }

    public void RestartGame(PointerEventData eventData)
    {
        Managers.Sound.Play("UIClick");
        Time.timeScale = 1f;
        Managers.Game.ReplayGame = true;
        GetObject((int)GameObjects.SettingPopup).SetActive(false);
        Managers.Scene.LoadScene(Define.Scene.Game);
    }

    public void GotoMain(PointerEventData eventData)
    {
        Managers.Sound.Play("UIClick");
        Time.timeScale = 1f;
        Managers.Scene.LoadScene(Define.Scene.Game);
    }

    public void PauseGame(PointerEventData eventData)
    {
        Managers.Sound.Play("UIClick");
        Time.timeScale = 0f;
        Get<Scrollbar>((int)Scrollbars.Sound).value = Managers.Sound.EffectVolumn;
        Get<Scrollbar>((int)Scrollbars.Music).value = Managers.Sound.BGMVolumn;
        GetObject((int)GameObjects.SettingPopup).SetActive(true);
    }
    public void ResumeGame(PointerEventData eventData)
    {
        Managers.Sound.Play("UIClick");
        Time.timeScale = 1f;
        GetObject((int)GameObjects.SettingPopup).SetActive(false);
    }
}
