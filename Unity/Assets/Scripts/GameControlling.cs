using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControlling : NetworkBehaviour
{


    public GameObject[] levels;

    [SyncVar]
    public GameObject gameovertext;

    [SyncVar]
    public GameObject latestLevel;

    [SyncVar]
    public bool endGameReached = false;

    GameObject[] players;

    [SyncVar]
    int activePlayers = 0;

    [SyncVar]
    int deathCounter = 0;

    [SyncVar]
    int nextlevelindex = 0;

    public override void OnStartServer()
    {
        base.OnStartServer();

        players = new GameObject[4];

        gameovertext.SetActive(false);

        //gameovertext = GameObject.Find("GameOverCanvas");

        //latestLevel = GameObject.Find("StartingLevel");

        Debug.Log("Starting GameflowController on Server");
    }

    public void addPlayer(GameObject player)
    {
        if (players[0] == null)
        {
            Debug.Log("Player 1 added to gameflow-controller");
            players[0] = player;
            activePlayers++;
            return;
        }
        else if (players[1] == null)
        {
            Debug.Log("Player 2 added to gameflow-controller");
            players[1] = player;
            activePlayers++;
            return;
        }
        else if (players[2] == null)
        {
            Debug.Log("Player 3 added to gameflow-controller");
            players[2] = player;
            activePlayers++;
            return;
        }
        else
        {
            Debug.Log("Player 4 added to gameflow-controller");
            players[3] = player;
            activePlayers++;
            return;
        }
    }

    public void removePlayer(int index)
    {
        if (players[index] != null)
        {
            players[index].SetActive(false);
            activePlayers--;
            deathCounter++;
            Debug.Log("Player deactivated on client and unspawned on server");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!endGameReached)
        {
            if (activePlayers <= 1 && deathCounter > 0) // Only one player left
            {
                Debug.Log("Game has ended only one player left");
                endGameReached = true;
                //gameovertext.gameObject.SetActive(true);
                // Instantiate game over text for all clients

                //NetworkServer.Spawn(gameovertext);
                Invoke("invokeEndGameCall", 1f);
            }
        }
    }

    public void createNextLevelIndex()
    {
        nextlevelindex = (int)Random.Range(0, levels.Length);
    }

    public int getNextLevelIndex()
    {
        return nextlevelindex;
    }

    void invokeEndGameCall()
    {
        string playername = "";

        for(int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                if (players[i].activeSelf) // Last surviving player
                {
                    playername = players[i].name;
                    Debug.Log("Winner is: " + playername);
                }
            }
        }

        RpcendGame(playername); // End the game on all clients
    }

    [ClientRpc] // Server tells all clients to execute this function
    void RpcendGame(string playername)
    {
        gameovertext.SetActive(true);
        gameovertext.GetComponentInChildren<Text>().text = "GAME! " + playername + " has won";
        Debug.Log("Game should end now");



        //Time.timeScale = 0; // Spiel freezed
    }
}
