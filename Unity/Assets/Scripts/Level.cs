using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : NetworkBehaviour
{

    private Timer timer;

    //[SyncVar(hook = "onLevelPositionChanged")]
    private Transform levelTransform;

    // Start is called before the first frame update
    public override void OnStartServer()
    {
        base.OnStartServer();

        levelTransform = gameObject.transform;
    }

    private void onLevelPositionChanged(Transform oldvalue, Transform newvalue)
    {
        levelTransform = newvalue;
    }

    
    private void Update()
    {

    }

    private void FixedUpdate()
    {
        if (timer == null)
        {
            try
            {
                timer = GameObject.Find("TimerCanvasNEU").GetComponent<Timer>();
            }
            catch (System.Exception e)
            {
                Debug.Log("KEIN TIMER");
                return;
            }
        }
        if (timer.levelMovementBlocked) { return; }

        //if (!isServer) { return; }
        CmdChangeLevelPosition();
    }


    void CmdChangeLevelPosition()
    {
        //levelTransform.position = Vector3.Lerp(levelTransform.position, levelTransform.position + new Vector3(0, -0.2f, 0), 0.2f);

        //levelTransform.

        //GameObject emptyGO = new GameObject();
        //Transform newTransform = emptyGO.transform;
        //newTransform.position = Vector3.Lerp(levelTransform.position, levelTransform.position + new Vector3(0, -0.2f, 0), 0.2f);

        //levelTransform = newTransform;

        RpcChangeLevelPositionOnClient();
    }

    //[ClientRpc]
    void RpcChangeLevelPositionOnClient()
    {
        transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0, -0.2f, 0), 0.2f);
        //Debug.Log(newpos);
        //levelTransform.position = newpos;
    }


}