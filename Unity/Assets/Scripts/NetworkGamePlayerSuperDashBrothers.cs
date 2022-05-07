using Mirror;
using UnityEngine;

public class NetworkGamePlayerSuperDashBrothers : NetworkBehaviour
{
    [SyncVar]
    private string displayName = "Loading...";

    private NetworkManagerSuperDashBrothers room;
    private NetworkManagerSuperDashBrothers Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerSuperDashBrothers;
        }
    }

    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);

        Room.GamePlayers.Add(this);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Debug.Log("Changing Scene...");
            //CmdUnfreezeGameTime();
            //nm = GameObject.Find("NetworkManager").GetComponent<NetworkManagerSuperDashBrothers>();
            Room.ServerChangeScene("Scene_Lobby");

        }
    }

    [Command]
    public void CmdUnfreezeGameTime()
    {
        RpcUnfreezeGameTime();
    }

    [ClientRpc]
    public void RpcUnfreezeGameTime()
    {
        Debug.Log("Unfreezing Game Time");
        AudioListener.pause = false;
        Time.timeScale = 1;
    }

    public override void OnNetworkDestroy()
    {
        Room.GamePlayers.Remove(this);
    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }

    public string getDisplayName()
    {
        return this.displayName;
    }
}

