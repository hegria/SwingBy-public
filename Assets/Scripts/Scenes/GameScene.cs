using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        Managers.Ads.ReadyAds();
        

        Application.targetFrameRate = 60;

        Managers.UI.ShowSceneUI<UI_GameScene>().gameObject.SetActive(false);

        SoundInit();


        if (Managers.Game.ReplayGame)
        {
            Managers.Game.Init();
            StartGame();
        }
        else
        {
            Managers.Game.Init();
            Managers.UI.ShowSceneUI<UI_MainScene>();
            Managers.Sound.Play("Main", Define.Sound.Bgm);
        }

        SceneType = Define.Scene.Game;
    }

    public override void Clear()
    {

    }

    void SoundInit()
    {
        Managers.Sound.GetOrAddAudioClip("Boom");
        Managers.Sound.GetOrAddAudioClip("Hit");
        Managers.Sound.GetOrAddAudioClip("SmallBoom");
        Managers.Sound.GetOrAddAudioClip("Boom");
        Managers.Sound.GetOrAddAudioClip("SmallSwing");
        Managers.Sound.GetOrAddAudioClip("Dead");
        Managers.Sound.GetOrAddAudioClip("Collide");
        Managers.Sound.GetOrAddAudioClip("Ouch");
        Managers.Sound.GetOrAddAudioClip("Equip");
        Managers.Sound.GetOrAddAudioClip("Shot");
        Managers.Sound.GetOrAddAudioClip("GameOver");
        Managers.Sound.GetOrAddAudioClip("Tunnel", Define.Sound.Movement);
        Managers.Sound.GetOrAddAudioClip("NormalEngine", Define.Sound.Movement);

    }


    #region IntroScene

    Vector3 DesiredCamera = new Vector3(0, 0, -10f);
    Vector3 ShipPos = new Vector3(0.15f, 0.15f);
    float goingup = 1f;

    public void StartGame()
    {
        if (Managers.UI.GetSceneUI<UI_MainScene>())
            Managers.UI.GetSceneUI<UI_MainScene>().Suicide();

        Managers.UI.GetSceneUI<UI_GameScene>().gameObject.SetActive(true);
        Managers.UI.GetSceneUI<UI_GameScene>().SudoInit();

        Managers.Sound.Stop(Define.Sound.Bgm);
        Managers.Sound.Play("Game", Define.Sound.Bgm);
        StartCoroutine("IntroSequence");
    }
  

    IEnumerator IntroSequence()
    {

        float nowtime = 0f;
        float nexttime = 2f;

        GameObject backgound = Managers.Game.Backgrounds.transform.GetChild(0).gameObject;

        Managers.Sound.Play("NormalEngine", Define.Sound.Movement);

        if (Ship.ship == null)
            yield return new WaitUntil(() => Ship.ship != null);

        while (true)
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 5.5f, 5 / 3f * Time.deltaTime);
            Camera.main.transform.Translate(new Vector3(0,1f*Time.deltaTime));
            Ship.ship.transform.localScale = Vector3.Lerp(Ship.ship.transform.localScale, ShipPos, 2f * Time.deltaTime);
            Ship.ship.transform.position = Ship.ship.transform.position + new Vector3(0, goingup * 0.75f * Time.deltaTime);
            backgound.transform.localScale = Vector3.Lerp(backgound.transform.localScale, Vector3.one, 2f * Time.deltaTime);
            
            nowtime += Time.deltaTime;
            if (nowtime >= nexttime)
                break;
            yield return null;
        }

        Ship.ship.StartGame();
        Managers.UI.GetSceneUI<UI_GameScene>().SetObjectsActive(true);
        Managers.Game.isGameStart = true;

        nowtime = 0f; 
        nexttime = 0.5f;
        while (true)
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 5.5f, 5/ 3f * Time.deltaTime);
            //Camera.main.transform.Translate(new Vector3(0, 1f * Time.deltaTime));
            Ship.ship.transform.localScale = Vector3.Lerp(Ship.ship.transform.localScale, ShipPos, 2f * Time.deltaTime);
            backgound.transform.localScale = Vector3.Lerp(backgound.transform.localScale, Vector3.one, 2f * Time.deltaTime);

            nowtime += Time.deltaTime;
            if (nowtime >= nexttime)
                break;
            yield return null;
        }
        backgound.transform.localScale = new Vector3(1.009171f, 1.009171f);

        Ship.ship.transform.localScale = ShipPos;

        Camera.main.orthographicSize = 5.5f;

        Managers.Tutorial.StartTutorial();

    }

    public void GameOver()
    {
        StopCoroutine("IntroSequence");
        Managers.Sound.Stop(Define.Sound.Movement);
        Managers.Sound.Stop(Define.Sound.Bgm);
        StartCoroutine("GameOverSound");

    }

    IEnumerator GameOverSound()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        Managers.Sound.Play("GameOver", volumn: 0.5f);
    }

    #endregion
}
