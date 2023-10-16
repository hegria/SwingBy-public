
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_FlightBtn : UI_GameButton
{
    // Start is called before the first frame update
    protected override void Onclick()
    {
        if (Ship.ship == null)
            return;

        Ship.ship.ClickChangeState();
    }

    protected override void OnPressUp()
    {
        Pressingtime = 0;
        if (Ship.ship == null)
            return;

        Ship.ship.UpChangeState();
    }

    float Pressingtime = 0;

    protected override void OnPressDown()
    {
        if (Ship.ship == null)
            return;
        Pressingtime = 0;
        switch (Ship.ship.state)
        {
            case Ship.State.Boosted:
                break;
            default:
                {
                    if (Ship.ship.state == Ship.State.Untargeted)
                    {
                        Ship.ship.FindPlanet();
                    }
                    //if (Ship.ship.state == Ship.State.Untargeted&&!Ship.ship.FindPlanet())
                    //    Ship.ship.state = Ship.State.Boosted;
                }
                break;
        }
    }
    protected override void OnPressing()
    {
        if (Ship.ship == null)
            return;
        Pressingtime += Time.deltaTime;

        if (Pressingtime <= 0.2f)
        {
            switch (Ship.ship.state)
            {
                case Ship.State.Boosted:
                    break;
                default:
                    {
                        if (Ship.ship.state == Ship.State.Untargeted)
                        {
                            Ship.ship.FindPlanet();
                        }
                        //if (Ship.ship.state == Ship.State.Untargeted&&!Ship.ship.FindPlanet())
                        //    Ship.ship.state = Ship.State.Boosted;
                    }
                    break;
            }
        }
    }
}
