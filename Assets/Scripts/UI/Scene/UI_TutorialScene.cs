using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class UI_TutorialScene : UI_Scene
{
    // Start is called before the first frame update

    enum GameObjects 
    {
        EntireObjects,
        Tutorial1,
        Tutorial2,
        Tutorial3,
        Tutorial4,
        Tutorial5,
        Tutorial6
    }


    int tutorialidx = 0;
    const int maxtutorial = 5;

    int nowtutorialidx = 1;

    int[] tutorialindice = { 3, 1, 2, 3, 2, 2 };

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        GetObject((int)GameObjects.EntireObjects).GetComponent<RectTransform>().offsetMax = new Vector2(0, -Util.getSafeTopArea() - Managers.Ads.getHeight());

        transform.SetAsFirstSibling();

        Get<GameObject>((int)GameObjects.EntireObjects).BindEvent(ProceedTutorial);

        for (int i =2; i<=(int)GameObjects.Tutorial6; i++)
        {
            Get<GameObject>(i).SetActive(false);
        }

        Canvas canvas = Util.GetOrAddComponent<Canvas>(gameObject);
        canvas.sortingOrder = 1;

    }

    private void Update()
    {

    }

    public void ProceedTutorial(PointerEventData eventData)
    {
        Managers.Sound.Play("UIClick");

        

        if (nowtutorialidx == tutorialindice[tutorialidx])
        {
            if (tutorialidx == maxtutorial)
            {
                Managers.Tutorial.EndTutorial();
                return;
            }
            Get<GameObject>((int)GameObjects.Tutorial1 + tutorialidx++).SetActive(false);
            Get<GameObject>((int)GameObjects.Tutorial1 + tutorialidx).SetActive(true);
            nowtutorialidx = 0;
        }

        nowtutorialidx++;
        Util.FindChild<LocalizeStringEvent>(Get<GameObject>((int)GameObjects.Tutorial1 + tutorialidx),recursive:true).StringReference.SetReference("Tutorial", $"Tutorial{tutorialidx + 1}_{nowtutorialidx}");
    }

}
