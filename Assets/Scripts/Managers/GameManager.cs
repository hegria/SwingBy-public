using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager
{

    // TODO
    // 로직호출을 좀 정리해야함.
    // 변수도 구조도로 만들어야하고
    float nextPlanet = 0f;
    float nextPath = 20f;
    float nextPathTerm = 50f;
    float nextBack = 18f;
    float nextBigObstacle = 25f;
    float nextBigTerm = 70f;

    public int nextAd = 0;

    const int Heighttoscore = 2;

    public bool ReplayGame = false;
    public bool isGameStart = false;

    [SerializeField]
    float _currentheight;

    [SerializeField]
    float _highheight;
    [SerializeField]
    float _highscore;
    [SerializeField]
    int _currentcomboscore = 0;
    [SerializeField]
    int _nowcombocount = 0;
    [SerializeField]
    float _comboscore = 0;
    [SerializeField]
    public const float ComboTime = 1f;
    [SerializeField]
    float _nowComboTime = 0f;
    float _score = 0;

    public Action<Define.ComboType> onCombo = null; // <- callback / Decopluing
    public List<Define.ComboType> _combolist = new List<Define.ComboType>();

    public float HighHeight {
        get
        {
            if (_currentheight >= _highheight)
            {
                PlayerPrefs.SetFloat("High", _currentheight);
                _highheight = _currentheight;
            }
            return _highheight;
        }
        set { _highheight = value; }
    }


    public float HighScore
    {
        get
        {
            if (_score >= _highscore)
            {
                PlayerPrefs.SetFloat("HighScore", _score);
                _highheight = _score;
            }
            return _highscore;
        }
        set { _highscore = value; }
    }
    public float CurrentHeight { get => _currentheight * 50; }

    public float realCurrentHeight { get => _currentheight; }

    public int CurrentComboscore
    {
        get { return _currentcomboscore; }
        set { _currentcomboscore = value; }
    }

    public int NowScore
    { 
        get
        {
            _score = _currentcomboscore + CurrentHeight * Heighttoscore;
            return (int)_score;
        }
    }
    public float NowComboTime
    {
        get { return _nowComboTime; }
        set { _nowComboTime = value; }
    }
    public int NowComboCount
    {
        get { return _nowcombocount; }
    }
    public float ComboScore
    {
        get { return _comboscore; }
        set { _comboscore = value;  }
    }
    Vector3 caposition;

    

    // Start is called before the first frame update

    [SerializeField]
    float _fuel;
    [SerializeField]
    float _maxFuel = 100.0f;

    public float  xmax = 4f;

    public float Fuel
    {
        get { return _fuel; }
        set
        {
            if (value >= MaxFuel)
            {
                _fuel = MaxFuel;
            }
            else if (value <= 0)
            {
                _fuel = 0;
            }
            else
            {
                _fuel = value;
            }
        }
    }

    //참조해야함
    public float MaxFuel
    {
        get { return _maxFuel; }
    }

    // Risk (Cost)
    // occuarance

    public void ComboUpdated(Define.ComboType comboType, bool isAlreadyDone = false)
    {
        switch (comboType)
        {
            case Define.ComboType.SmallSwing: // +a 진행되는 동안 2 * deltatime
                _comboscore += 50;
                break;
            case Define.ComboType.BigSwing:  // -> +a 진행되는 동안 4 * deltatime
                _comboscore += 200;
                break;
            case Define.ComboType.Comet:
                _comboscore += 500;
                break;
            case Define.ComboType.CrackAstro:
                _comboscore += 100;
                break;
            case Define.ComboType.CrackPannel: // 4개부수는거? : 미구현
                _comboscore += 50;
                break;
            case Define.ComboType.SpaceTunnel: // -> +a 진행되는 동안 3 * deltatime
                _comboscore += 50;
                break;
            case Define.ComboType.Graze:
                _comboscore += 300;
                break;
            case Define.ComboType.Shooting:
                _comboscore += 100;
                break;
            default:
                break;
        }


        _nowComboTime = ComboTime;

        if (isAlreadyDone)
        {

        }
        else
        {
            _nowcombocount += 1;
            if (onCombo != null)
                onCombo(comboType);
        }

        _combolist.Add(comboType);

    }

    public GameObject Planets;
    public GameObject Obstacles;
    public GameObject Comets;
    public GameObject Paths;
    public GameObject Backgrounds;
    public GameObject Explosions;

    bool canComet;

    public void Init()
    {
        ReplayGame = false;
        _nowcombocount = 0;
        _currentcomboscore = 0;
        _comboscore = 0;
        _combolist = new List<Define.ComboType>();
        caposition = new Vector3(0, 0, -10f);
        _currentheight = .0f;
        nextPlanet = UnityEngine.Random.Range(3f, 4f) + 1f;
        nextBack = 38f;
                nextBigObstacle = 25f;
        Planets = GameObject.Find("Planets");
        Obstacles = GameObject.Find("Obstacles");
        Comets = GameObject.Find("Comets");
        Paths = GameObject.Find("Pathways");
        Backgrounds = GameObject.Find("Backgrounds");
        Explosions = GameObject.Find("Explosions");
        GenPlanet();
        isGameStart = false;
        _fuel = MaxFuel;
        _highheight = PlayerPrefs.GetFloat("High");
        _highscore = PlayerPrefs.GetFloat("HighScore");
        nextPath = UnityEngine.Random.Range(nextPathTerm -10f, nextPathTerm + 10f );
        canComet = false;
        onCombo = null;
    }

    public void OnUpdate()
    {

        if (Ship.ship == null || !isGameStart)
        {
            return;
        }

        if(_nowComboTime >= 0f)
        {
            _nowComboTime -= Time.deltaTime;
        }

        if (_nowcombocount >= 1 && _nowComboTime <= 0f)
        {
            _nowComboTime = 0f;
            _currentcomboscore += (int)_comboscore *  _nowcombocount;
            _nowcombocount = 0;
            _comboscore = 0;
            Ship.ship.firstShooted = false;
            _combolist.Clear();
            if(onCombo != null)
                onCombo(Define.ComboType.ComboEnd);
        }

        float y = Ship.ship.transform.position.y;
        if (_currentheight < y)
        {
            _currentheight = y;

            caposition.y = _currentheight + 1.5f;
        }
        if (isGameStart)
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position , caposition , 10f * Time.deltaTime);

        if (_currentheight >= nextPlanet - 9f)
        {
            GenPlanet();
        }
        if (_currentheight >= nextPath - 9f)
        {
            GenPath();
        }

        if (_currentheight >= nextBigObstacle - 9f)
        {
            GenBigObstacle();
        }
        if (_currentheight >= nextBack - 27f)
        {
            GenBack();
        }

    }


    private void GenBack()
    {
        GameObject go = Managers.Resource.Instantiate("Background", Planets.transform);
        go.transform.position = new Vector3(0, nextBack);
        nextBack += 40f;
        go.transform.parent = Backgrounds.transform;
    }

    void GenPlanet()
    {
        GameObject go = Managers.Resource.Instantiate("Planet", Planets.transform);
        go.transform.position = new Vector3(UnityEngine.Random.Range(-2f, 2f), nextPlanet);
        nextPlanet += UnityEngine.Random.Range(4f, 7f);


        int a = UnityEngine.Random.Range(1,4);
        if (a == 3)
        {
            a = 4;
        }

        Vector3 pos;
        while (true)
        {
            pos = new Vector3(UnityEngine.Random.Range(-2f, 2f), nextPlanet + 2.5f + UnityEngine.Random.Range(-2f, 1f));

            if ((pos - go.transform.position).magnitude >= 2.5f)
            {
                break;
            }
        }

        GameObject obstacle = Managers.Resource.Instantiate($"ObstacleStack{a}", Obstacles.transform);
        obstacle.transform.position = pos;
        obstacle.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-90f, 90f));

        Vector3 beforepos = pos;

        a = UnityEngine.Random.Range(1, 4);
        if (a == 3)
        {
            a = 4;
        }

        while (true)
        {
            pos = new Vector3(UnityEngine.Random.Range(-2f, 2f), nextPlanet - 3f + UnityEngine.Random.Range(-2f, 2f));

            if ((pos - go.transform.position).magnitude >= 2.5f && (beforepos - pos).magnitude >= 4f)
            {
                break;
            }
        }
        obstacle = Managers.Resource.Instantiate($"ObstacleStack{a}", Obstacles.transform);
        obstacle.transform.position = pos;
        obstacle.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-90f, 90f));
        
        if (canComet)
        {
            
            GameObject fuel = Managers.Resource.Instantiate($"Comet{UnityEngine.Random.Range(1,3)}", Comets.transform);
            fuel.transform.position =
                new Vector3(UnityEngine.Random.Range(-3f, 3f), nextPlanet + UnityEngine.Random.Range(-2f, 2f));
        }
        canComet = !canComet;

        
    }

    public bool WillIDie(Vector3 pos)
    {
        return pos.x >= Managers.Game.xmax || pos.x <= -Managers.Game.xmax || pos.y <=Camera.main.transform.position.y - 10.0f;
    }

    void GenBigObstacle()
    {
        GameObject go = Managers.Resource.Instantiate("BigObstacle", Obstacles.transform);
        go.transform.position = new Vector3(UnityEngine.Random.Range(-0.5f,0.5f), nextBigObstacle);
        go.GetComponent<Obstacle>().SetMovofBigObstacle();
        nextBigObstacle += UnityEngine.Random.Range(nextBigTerm, nextBigTerm + 30f);
    }

    void GenPath()
    {
        int a = UnityEngine.Random.Range(1, 4);

        Vector3 pos;
        while (true)
        {
            pos = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), nextPath + UnityEngine.Random.Range(-2f, 2f));
            Vector3 plus = new Vector3(0, 2.03f);
            bool isokay = true;
            for (int i =0; i< Planets.transform.childCount; i++)
            {
                if ((pos - Planets.transform.GetChild(i).transform.position + plus).magnitude <= 2)
                    isokay = false;
            }
            if (isokay)
                break;
        }
        
        GameObject go = Managers.Resource.Instantiate($"Paths/Path{a}", Paths.transform);


        go.transform.GetChild(0).GetChild(0).GetComponent<PathCreation.Examples.RoadMeshCreator>().TriggerUpdate();
        go.transform.position = pos;
        nextPath += UnityEngine.Random.Range(nextPathTerm, nextPathTerm + 20f);
    }


    public void GenExplosion(string path, Vector3 position)
    {
        GameObject go = Managers.Resource.Instantiate($"Explosion/{path}", Managers.Game.Explosions.transform);
        go.transform.position = position;
    }

    
}
