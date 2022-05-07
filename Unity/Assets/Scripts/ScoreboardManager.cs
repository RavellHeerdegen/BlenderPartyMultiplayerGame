using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreboardManager : NetworkBehaviour
{
    [SyncVar(hook = nameof(onNumPlayerLeftChange))]
    private int numPlayersLeft;
    //private bool freezeTime = false;

    public int NumPlayersLeft
    {
        get
        {
            return numPlayersLeft;
        }
        /*set
        {
            numPlayersLeft = value;
        }*/
    }
    [Command]
    public void Cmd_SetNumPlayersLeft(int value)
    {
        numPlayersLeft = value;
    }

    public override void OnStartServer()
    {
        //numPlayersLeft = Room.RoomPlayers.Count + 1;
        //Debug.Log("Starting the round with " + numPlayersLeft + " players!");
        //base.OnStartServer();
    }

    private NetworkManagerSuperDashBrothers room;
    private NetworkManagerSuperDashBrothers Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerSuperDashBrothers;
        }
    }

    public void onNumPlayerLeftChange(int oldValue, int newValue)
    {
        numPlayersLeft = newValue;
        Debug.Log("in onNumPlayerLeftChange, old value was " + oldValue + " new Value is: " + newValue );
        if (newValue <= 0)
        {
            //freezeTime = true;
            Invoke("freezeTimeScale", 1f);
        }
    }

    private void freezeTimeScale()
    {
        Debug.Log("Freezing time...");
        Time.timeScale = 0;
    }

    /*
    IEnumerator ScaleTime(float start, float end, float time)     //not in Start or Update
    {
        float lastTime = Time.realtimeSinceStartup;
        float timer = 0.0f;

        while (timer < time)
        {
            Time.timeScale = Mathf.Lerp(start, end, timer / time);
            timer += (Time.realtimeSinceStartup - lastTime);
            lastTime = Time.realtimeSinceStartup;
            yield return null;
        }

        Time.timeScale = end;
    }


    void Update()
    {
        if (freezeTime && Time.timeScale == 1)
        {
            Debug.Log("Starting to freeze time");
            StartCoroutine(ScaleTime(1.0f, 0.0f, 1.0f));
        }
        //if (freezeTime && Time.timeScale == 0)
        //{
        //    StartCoroutine(ScaleTime(0.0f, 1.0f, 1.0f));
        //}
    }
    */
}
