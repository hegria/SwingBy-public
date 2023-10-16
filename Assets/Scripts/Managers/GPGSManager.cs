using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GPGSManager
{
    // Start is called before the first frame update


    public bool logined = false;

    public void Init()
    {
        PlayGamesPlatform.Activate();
        Social.localUser.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(bool success)
    {
        Debug.Log(success);
        if (success == true)
        {
            // Continue with Play Games Services
            logined = true;
        }
        else
        {

        }
    }
    internal void ProcessAuthenticationWithShowLeaderBoard(bool success)
    {
        if (success == true)
        {
            // Continue with Play Games Services
            logined = true;
            Social.ShowLeaderboardUI();
        }
        else
        {

        }
    }

    public void Clear()
    {

    }

    public void UploadScore()
    {
        if (logined)
            Social.ReportScore(Managers.Game.NowScore, "CgkItJLvmOQUEAIQAQ", (bool success) => { });
    }


    public void ShowLeaderBoard()
    {
        if (logined)
            Social.ShowLeaderboardUI();
        else
            Social.localUser.Authenticate(ProcessAuthenticationWithShowLeaderBoard);
    }
}
