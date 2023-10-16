using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ship : MonoBehaviour
{



#region Defines

    public enum State {
        Targeted,
        Untargeted,
        Boosted,
        Dash,
        OnPath,
        Dead
    }

    

    public enum BoostDir
    {
        Left,
        Middle,
        Right,
    }

    public enum MouseDir
    {
        Left,
        Middle,
        Right,
    }
    #endregion


    private static Ship _ship = null;

    public static Ship ship
    {
        get
        {
            if (null == _ship)
            {
                return null;
            }
            return _ship;
        }
    }


    [SerializeField]
    float _angleSpeed;
    [SerializeField]
    float _normalSpeed;
    [SerializeField]
    float _gravity;
    [SerializeField]
    float _fuelDesc;
    [SerializeField]
    float _maxSpeed;
    [SerializeField]
    float _minSpeed;
    [SerializeField]
    float _slowrate = 0.5f;
    [SerializeField]
    float MaxSwing = 2f;

    // TODO
    float _beforespeed;
    [SerializeField]
    float _nowAngleSpeed;
    float _swingGauge;

    public GameObject target_planet;

    GameObject shild;

    
    public float SwingGauge
    {
        get { return _swingGauge; }
        set
        {
            if (value >= MaxSwing)
            {
                _swingGauge = MaxSwing;
            }
            else if (value <= 0)
            {
                _swingGauge = 0;
            }
            else
            {
                _swingGauge = value;
            }
        }
    }

    public float Speed
    {
        get { return _normalSpeed; }
        set 
        { 
            if(value >= _maxSpeed)
            {
                _normalSpeed = _maxSpeed;
            }
            else if (value <= _minSpeed)
            {
                _normalSpeed = _minSpeed;
            }
            else
                _normalSpeed = value; 
        }
    }

    public float MaxSpeed { get { return _maxSpeed; } }

    // Componet
    Material material;
    PathCreation.Examples.PathFollower path;
    SpriteRenderer sprite;


    //내부 변수
    Vector3 uptowrad;
    bool clockwise;
    int dir;

    public bool firstShooted;

    bool shootingEnabled;
    

    const float shooting_delay = 0.4f;
    const float shootingFulltime = 5f;
    float nowShootingTime = 0f;

    const float docking_delay = 0.5f;
    float docked_time = 0.5f;



    State _state = State.Untargeted;
    State beforestate = State.Untargeted;
    BoostDir _boostdir = BoostDir.Middle;

    public State state
    {
        get { return _state; }
        set
        {
            // Outstate를 따로 작업하는것도 좋을듯함.

            beforestate = _state;
            _state = value;
            float distance = FindDistance();

            switch (_state)
            {
                case State.Targeted:
                    _beforespeed = Speed;


                    //line.enabled = true;
                    //line.SetPositions(new Vector3[] { transform.position, transform.position + transform.up * 4.0f });

                    material.color = Color.yellow;
                    // TODO 각도 변환 액션 Lerp Angle Speed
                    _nowAngleSpeed = Mathf.Rad2Deg * Speed / 2;
                    //Debug.Log($"Now Angle : {_nowAngleSpeed}");
                    // 코루틴을 쓰지않을까 싶음 / Update 문

                    Managers.Game.ComboUpdated(Define.ComboType.BigSwing);
                    docked_time = .0f;
                    Managers.Game.Fuel += 20f;
                    break;
                case State.Untargeted:
                    // TODO 속도 복귀. 코루틴을 통해구현
                    material.color = Color.yellow;
                    if (beforestate == State.Targeted)
                    {
                        Speed = _beforespeed;
                        Managers.Game.ComboScore += 50;
                    }
                    target_planet = null;
                    break;
                case State.Boosted:
                    material.color = Color.red;
                    break;
                case State.Dash:
                    Managers.Sound.Play("SwingBooster");
                    shild.SetActive(true);
                    material.color = new Color(1.0f, 0.65f, 0f);
                    //material.color = Color.red;
                    StartCoroutine("Dash");
                    break;
                case State.Dead:
                    Managers.Game.NowComboTime = 0.0f;
                    break;
            }
        }
    }

    public BoostDir boostDir
    {
        get { return _boostdir; }
        set
        {
            _boostdir = value;
        }
    }


    void Awake()
    {
        if (null == _ship)
        {
            _ship = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        Init();
    }

    public void Init()
    {
        material = Managers.Resource.Load<Material>("Art/Material/New Material");
        path = GetComponent<PathCreation.Examples.PathFollower>();
        sprite = GetComponent<SpriteRenderer>();
        path.enabled = false;
        shootingEnabled = false;
        _swingGauge = 1.0f;
        shild = transform.GetChild(0).gameObject;
        shild.SetActive(false);
    }

    public void StartGame()
    {
        //FindPlanet();
        //Managers.Input.MouseAction += ChangeState;
    }

    // Update is called once per frame
    bool gameovered = false;

    void Update()
    {
        if (!Managers.Game.isGameStart)
            return;
        switch (_state)
        {
            case State.Targeted:
                docked_time += Time.deltaTime;
                Managers.Game.Fuel -= 0.5f * _fuelDesc * Time.deltaTime;

                Managers.Game.ComboScore += 4 * Time.deltaTime;
                Managers.Game.NowComboTime = 1.0f;
                dir = 1;
                if (clockwise)
                    dir = -1;
                //TODO 속도를 수렴시켜야한다.

                _nowAngleSpeed = Mathf.Lerp(_nowAngleSpeed, _angleSpeed, 2 * Time.deltaTime);

                transform.RotateAround(target_planet.transform.position, transform.forward, dir *
                    _nowAngleSpeed * Time.deltaTime);
                Vector3 goingToward = Vector3.Cross(dir * (target_planet.transform.position - transform.position), transform.forward).normalized;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, goingToward), 4 * Time.deltaTime);
                // 빨려들어가는거
                //transform.position = Vector3.Lerp(transform.position, target_planet.transform.position, _gravity * Time.deltaTime);
                break;
            case State.Untargeted:
                docked_time += Time.deltaTime;
                Managers.Game.Fuel -= _fuelDesc * Time.deltaTime;
                Speed -= _slowrate *Time.deltaTime;
                transform.Translate(Speed * Vector3.up * Time.deltaTime);
                break;
            case State.Boosted:

                CheckMouseDIr();
                Managers.Game.Fuel -= 3f * _fuelDesc * Time.deltaTime;
                {
                    Quaternion lookto;

                    switch (_boostdir)
                    {
                        case BoostDir.Left:
                            lookto = Quaternion.LookRotation(transform.forward, -transform.right);
                            transform.rotation = Quaternion.Lerp(transform.rotation, lookto, Time.deltaTime);
                            break;
                        case BoostDir.Right:
                            lookto = Quaternion.LookRotation(transform.forward, transform.right);
                            transform.rotation = Quaternion.Lerp(transform.rotation, lookto, Time.deltaTime);
                            break;
                        case BoostDir.Middle:
                            Speed += 0.5f * Time.deltaTime;
                            break;
                    }

                }   
                transform.Translate(Speed * Vector3.up * Time.deltaTime);
                break;
            case State.Dash:
                docked_time += Time.deltaTime;
                Managers.Game.Fuel -= _fuelDesc * Time.deltaTime;
                transform.Translate(Speed * Vector3.up * Time.deltaTime);
                break;
            case State.Dead:
                transform.Translate(0.25f * Vector3.up * Time.deltaTime);
                break;
            case State.OnPath:
                Managers.Game.ComboScore += 2f * Time.deltaTime;
                Managers.Game.NowComboTime = 1.0f;
                break;

        }

        if (transform.position.x >= Managers.Game.xmax || transform.position.x <= -Managers.Game.xmax || Managers.Game.Fuel <= 0
            || Managers.Game.realCurrentHeight >= transform.position.y + 5f)
        {
            if (!gameovered)
            {
                //TODO 바꿔야함
                gameovered = true;
                Managers.Input.MouseAction -= ChangeState;
                GetComponent<TrailRenderer>().enabled = false;
                Util.FindChild<TrailRenderer>(gameObject, "Left").enabled = false;
                Util.FindChild<TrailRenderer>(gameObject, "Right").enabled = false;
                // 양옆 불도 끄기
                Time.timeScale = 0.3f;
                ((GameScene)(Managers.Scene.CurrentScene)).GameOver();
                StartCoroutine("GameOver");
            }

        }

        if (nowShootingTime > 0)
        {
            nowShootingTime -= Time.deltaTime;
            
            if (nowShootingTime <= 0)
            {
                nowShootingTime = 0;
                shootingEnabled = false;
                StopCoroutine("Shooting");
            }
        }

        
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSecondsRealtime(0.3f);


        Managers.GPGS.UploadScore();
        Managers.UI.GetSceneUI<UI_GameScene>().SetGameOver();
        if (Managers.Game.Fuel <= 0)
        {
            state = State.Dead;
        }
        Managers.Game.nextAd++;
        if (Managers.Game.nextAd >= 4)
        {
            Managers.Ads.ShowAd();
            Managers.Game.nextAd = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Obstacle"))
        {
            collision.gameObject.GetComponent<Obstacle>().SetHitted();
            
            if (state == State.OnPath)
            {

            }
            else if (collision.GetComponent<BigObstacle>())
            {
                Managers.Sound.Play("Dead");
                Managers.Game.GenExplosion("SmallExplosion0.25", transform.position);
                sprite.enabled = false;
                Managers.Game.Fuel = 0;
                _normalSpeed = 0f;
            }
            else if (state == State.Dash ||  state == State.Targeted)
            {
                Managers.Sound.Play("Collide");
                Managers.Sound.Play("SmallBoom");
                Managers.Game.GenExplosion("SmallExplosion0.25", collision.gameObject.transform.position);

                Destroy(collision.gameObject);
                Speed += 0.3f;
                Managers.Game.Fuel += 5f;
                Ship.ship.SwingGauge += 0.05f;

                bool isAlreadyBroken = false;

                if (collision.transform.parent.GetComponent<ObstacleParent>())
                {
                    if (!collision.transform.parent.GetComponent<ObstacleParent>().isAlreadyBroken)
                    {
                        collision.transform.parent.GetComponent<ObstacleParent>().isAlreadyBroken = true;
                    } else
                    {
                        isAlreadyBroken = true;
                    }
                    
                } 

                Managers.Game.ComboUpdated(Define.ComboType.CrackAstro, isAlreadyBroken);
            }
            else
            {
                // 처맞는거
                Speed -= 0.5f;
                Managers.Game.Fuel -= 5f;
                Managers.Game.NowComboTime = 0;
                Managers.Sound.Play("Ouch");
                StartCoroutine("Ouch");
            }
            return;
        }

        if (collision.tag.Equals("Comet"))
        {
            Managers.Game.Fuel += 10f;
            Destroy(collision.gameObject);
            EnableShooting();
            Managers.Sound.Play("Equip",volumn:0.7f);
            Managers.Game.ComboUpdated(Define.ComboType.Comet);
            return;
        }

        if (collision.tag.Equals("Path"))
        {
            path.pathCreator = collision.transform.GetChild(0).gameObject.GetComponent<PathCreation.PathCreator>();
            path.speed = Speed;
            path.enabled = true;
            if (state == State.Dash)
            {
                StopCoroutine("Dash");
                Speed -= 2.0f;
            }
            state = State.OnPath;
            path.OnEndofPath -= EndofPaths;
            path.OnEndofPath += EndofPaths;

            Managers.Sound.Play("Tunnel", Define.Sound.Movement);
            
            //진입시에 바꿔야함.
            Managers.Game.ComboUpdated(Define.ComboType.SpaceTunnel);
        }
        
    }

    //state 변경
    void EndofPaths()
    {
        state = State.Untargeted;
        
        path.OnEndofPath -= EndofPaths;
        path.enabled = false;
        path.pathCreator = null;

        Managers.Sound.Play("NormalEngine", Define.Sound.Movement);

    }


    public bool FindPlanet()
    {
        if (docked_time < docking_delay)
            return false;

        //TODO 너무 가까운데에서 지랄나는거 추가
        if (SwingGauge < 1.0f)
        {
            return false;
        }


        
        if (target_planet == null)
            return false;

        Vector3 tempvec = Vector3.Cross(target_planet.transform.position - transform.position, transform.up).normalized;
        if (Vector3.Dot(Vector3.forward, tempvec) < 0)
            clockwise = false;
        else
            clockwise = true;
        state = State.Targeted;
        SwingGauge -= 1f;

        return true;
    }

    public void EnableShooting()
    {
        StopCoroutine("Shooting");
        shootingEnabled = true;
        firstShooted = false;
        nowShootingTime = shootingFulltime;
        StartCoroutine("Shooting");
    }

    public void BrokenObjectByShooting(bool isPlanet =false)
    {

        Managers.Game.ComboUpdated(Define.ComboType.Shooting, firstShooted);
        Managers.Game.Fuel += 10f;
                Ship.ship.SwingGauge += 0.05f;

        if (isPlanet)
        {
            Ship.ship.SwingGauge += 0.05f;
            Managers.Game.Fuel += 10f;
        }
        if(!firstShooted)
        {
            firstShooted = true;
        }
    }

    IEnumerator Shooting()
    {
        while (shootingEnabled)
        {
            if ( state != State.OnPath)
            {
                
                GameObject bullet = Managers.Resource.Instantiate("Bullet");
                bullet.transform.position = transform.position;
                bullet.transform.rotation = transform.rotation;
                Managers.Sound.Play("Shot");
            }
            yield return new WaitForSeconds(shooting_delay);
        }
    }

    IEnumerator Dash()
    {
        float beforespeed = Speed;

        if (beforestate != State.Targeted)
            Managers.Game.Fuel = Mathf.Max(Managers.Game.Fuel - 10f, 5f); ;
        Speed += 3.0f;
        yield return new WaitForSeconds(0.5f);
        Speed -= 3.0f;
        state = State.Untargeted;
        shild.SetActive(false);
    }

    IEnumerator Ouch()
    {
        sprite.color =  new Color(1.0f,1.0f,1.0f,0.7f);
        yield return new WaitForSeconds(0.1f);
        sprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        yield return new WaitForSeconds(0.1f);
        sprite.color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
        yield return new WaitForSeconds(0.1f);
        sprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        yield return new WaitForSeconds(0.1f);
    }


#region Click액션
    void ChangeState(Define.MouseEvent mouseEvent)
    {
        if (mouseEvent == Define.MouseEvent.Click)
        {
            Managers.UI.MakeWorldSpaceUI<UI_Clicked>(Camera.main.transform).SetDir(Input.mousePosition);

            ClickChangeState();
        }
        //else if (mouseEvent == Define.MouseEvent.Press)
        //{
        //    switch (_state)
        //    {
        //        case State.Boosted:
        //            break;
        //        default:
        //            state = State.Boosted;
        //            break;
        //    }
        //}
        else
        {
            UpChangeState();
        }
    }

    public void ClickChangeState()
    {
        switch (_state)
        {
            case State.Untargeted:
                {
                    if(Managers.Game.Fuel >= 5.5f)
                        state = State.Dash;
                }
                break;
            default:
                break;
             
        }
    }

    public void UpChangeState()
    {
        switch (_state)
        {
            case State.Targeted:
                // 반구에서 씹기
                state = State.Dash;
                break;
            //case State.Boosted:
            //    state = State.Untargeted;
            //    break;
            default:
                break;
        }
    }

    public void DownChangeState() { }


    // Rotation을 살짝만 바꾸는거
    public void clickRotation(BoostDir boost)
    {
        if (state == State.OnPath )
        {
            path.Escape();
        }

        if (boost == BoostDir.Left)
        {
            transform.rotation = transform.rotation * Quaternion.Euler(0, 0, 15);
        }
        else
        {
            transform.rotation = transform.rotation * Quaternion.Euler(0, 0, -15);

        }
    }
    public void PressingRotation(BoostDir boost)
    {
        if (boost == BoostDir.Left)
        {
            transform.rotation = transform.rotation * Quaternion.Euler(0, 0, 80f * Time.deltaTime);
        }
        else
        {
            transform.rotation = transform.rotation * Quaternion.Euler(0, 0, -80f * Time.deltaTime);

        }
    }

    #endregion




    #region 편의함수
    void CheckMouseDIr()
    {
        Vector3 mousepos = Input.mousePosition;
        float triwidth = Screen.width / 3f;
        if (mousepos.x >= 2 * triwidth)
        {
            _boostdir = BoostDir.Right;
        }
        else if (mousepos.x >= triwidth)
        {
            _boostdir = BoostDir.Middle;
        }
        else
        {
            _boostdir = BoostDir.Left;
        }
    }
    float FindDistance()
    {
        if (target_planet == null)
            return .0f;
        return Vector3.Distance(transform.position, target_planet.transform.position);
    }
#endregion

}
