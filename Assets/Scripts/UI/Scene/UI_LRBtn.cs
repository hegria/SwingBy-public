using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LRBtn : UI_GameButton
{
    enum Btn
    {
        Left,
        Right
    }

    [SerializeField]
    Btn btn = Btn.Left;


    // Start is called before the first frame update

    protected override void Onclick()
    {
        // TODO 각도 살짝 이동


        if (Ship.ship == null)
            return;

            switch (btn)
        {
            case Btn.Left:
                Ship.ship.clickRotation(Ship.BoostDir.Left);
                break;
            case Btn.Right:
                Ship.ship.clickRotation(Ship.BoostDir.Right);
                break;
        }
    }
    protected override void OnPressUp()
    {
        if(Ship.ship!=null)
            Ship.ship.UpChangeState();
    }

    protected override void OnPressDown()
    {
        if (Ship.ship == null)
            return;
        //합쳐야함.
        switch (btn)
        {
            case Btn.Left:
                Ship.ship.clickRotation(Ship.BoostDir.Left);
                break;
            case Btn.Right:
                Ship.ship.clickRotation(Ship.BoostDir.Right);
                break;
        }
    }
    protected override void OnPressing()
    {
        if (Ship.ship == null)
            return;
        switch (btn)
        {
            case Btn.Left:
                Ship.ship.PressingRotation(Ship.BoostDir.Left);
                break;
            case Btn.Right:
                Ship.ship.PressingRotation(Ship.BoostDir.Right);
                break;
        }
    }

    void Setdir()
    {
        switch (btn)
        {
            case Btn.Left:
                Ship.ship.boostDir = Ship.BoostDir.Left;
                break;
            case Btn.Right:
                Ship.ship.boostDir = Ship.BoostDir.Right;
                break;
        }
    }
}
