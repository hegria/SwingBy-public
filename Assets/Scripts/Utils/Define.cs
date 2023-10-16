using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum Scene
    {
        Unknown,
        Login,
        Lobby,
        Game,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        Movement,
        MaxCount
    }

    public enum UIEvent
    {
        Click,
        Drag,
        Down,
        Up,
    }

    public enum MouseEvent
    {
        Press,
        Click,
        None
    }

    public enum ComboType
    {
        SmallSwing,
        BigSwing,
        CrackAstro,
        CrackPannel,
        SpaceTunnel,
        Comet,
        Graze,
        Shooting,
        ComboEnd,
    }


}
