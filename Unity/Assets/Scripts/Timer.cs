using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : NetworkBehaviour
{

    [SyncVar]
    public int countdownStartValue = 10;

    [SyncVar]
    public bool movementBlocked = true;

    [SyncVar]
    public bool levelMovementBlocked = true;

    public Text timerUI;

    [SyncVar(hook = "onTimerTextChange")]
    private string timertext;

    [Server]
    public void startCountdown()
    {
        Debug.Log("Starting countdown");

        countdownTimer();
    }

    void onTimerTextChange(string oldvalue, string newvalue)
    {
        timerUI.text = newvalue;
    }

    [Server]
    void countdownTimer()
    {
        if (countdownStartValue > 0)
        {
            TimeSpan timespan = TimeSpan.FromSeconds(countdownStartValue);

            timertext = "" + timespan.Seconds;

            countdownStartValue--;
            Invoke("countdownTimer", 1.0f);
        }
        else
        {
            movementBlocked = !movementBlocked;
            timertext = "GO!";

            Invoke("deactivateTimerCanvas", 2.0f);
        }
    }

    [Server]
    void deactivateTimerCanvas()
    {
        timertext = "";
        timerUI.text = timertext;

        Invoke("deactivateLevelMovementBlocked", 1.0f);
    }

    [Server]
    void deactivateLevelMovementBlocked()
    {
        levelMovementBlocked = false;
    }

    //[ClientRpc]
    //void RpccountdownTimer()
    //{
    //    Debug.Log("Bin drin in Rpc");

    //    if (countdownStartValue > 0)
    //    {
    //        TimeSpan timespan = TimeSpan.FromSeconds(countdownStartValue);

    //        timerUI.text = "" + timespan.Seconds;
    //        countdownStartValue--;
    //        Invoke("RpccountdownTimer", 1.0f);
    //    }
    //    else
    //    {
    //        timerUI.text = "GO!";

    //    }
    //}

}
