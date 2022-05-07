using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollisionDetectorPlayer : NetworkBehaviour
{

    public GameObject death_animation;
    public string networkGamePlayerName;

    [SerializeField] private GameObject deathsound_Prefab;
    [SerializeField] private GameObject winsound_Prefab;
    [SerializeField] private GameObject Scoreboard_Prefab;

    // Muss übers Netzwerk passieren
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Rotorblade")
        {
            Debug.Log("AUUUUUUUUUUUUUUUUUUUUUUUUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");

            // Muss auf allen Clients passieren an alle Clients gesendet werden

            if (hasAuthority)
            {
                CmdRemovePlayer(gameObject);
            }
        }
    }

    [Command]
    void CmdRemovePlayer(GameObject gameobj)
    {
        // Deactivate player on all clients, but not delete it
        GameObject death_anim = Instantiate(death_animation);
        death_anim.transform.position = gameobj.transform.position;

        Debug.Log("Vorher noch " + Room.numOfPlayersLeftServer + " im Spiel!");
        Room.numOfPlayersLeftServer--;
        Debug.Log("Entferne Spieler, noch " + Room.numOfPlayersLeftServer + " von " + Room.totalNumOfPlayers + " im Spiel!");
        //CollisionDetectorPlayer[] players = GameObject.Find("Player(Clone)").GetComponents<CollisionDetectorPlayer>();
        for (int i = 0; i < Room.totalNumOfPlayers; i++)
        {
            if (Room.Scoreboard[i].Key != networkGamePlayerName) // && that player is still active
            {
                Room.Scoreboard[i] = new KeyValuePair<string, int>(Room.Scoreboard[i].Key, Room.Scoreboard[i].Value + 1);
            }
        }

        string debugStr = "";
        for (int i = 0; i< Room.Scoreboard.Count; i++)
        {
            debugStr += Room.Scoreboard[i];
        }
        Debug.Log("Scoreboard so far: " + debugStr);


        bool freezeGame = false;
        if ((Room.numOfPlayersLeftServer == 1 && Room.totalNumOfPlayers > 1) || (Room.totalNumOfPlayers == 1 && Room.numOfPlayersLeftServer == 0))
        {
            //Play winning sound!
            if(Room.totalNumOfPlayers > 1)
            {
                GameObject winsound_object = Instantiate(winsound_Prefab);
                NetworkServer.Spawn(winsound_object);
            }
            freezeGame = true;
        }

        NetworkServer.Spawn(death_anim); // Spawn death animation on all clients

        // Play death sound
        GameObject deathsound_object = Instantiate(deathsound_Prefab);
        NetworkServer.Spawn(deathsound_object);

        // Make Scoreboard serializable
        string[] names = new string[4];
        int[] scores = new int[4];

        for (int i = 0; i < Room.Scoreboard.Count; i++)
        {
            names[i] = Room.Scoreboard[i].Key;
            scores[i] = Room.Scoreboard[i].Value;
        }

        // Remove player from all clients
        RpcremovePlayer(gameobj, freezeGame, names, scores, Room.totalNumOfPlayers);
    }

    [ClientRpc]
    void RpcremovePlayer(GameObject gameobj, bool freezeGame, string[] names, int[] scores, int totalNumOfPlayers)
    {
        Room.totalNumOfPlayers = totalNumOfPlayers;

        if (Room.Scoreboard.Count == 0)
        {
            initializeRoomScoreboard();
        }

        for (int i = 0; i < totalNumOfPlayers; i++)
        {
            Room.Scoreboard[i] = new KeyValuePair<string, int>(names[i], scores[i]);
        }


        gameobj.SetActive(false);
        if (freezeGame)
        {
            Invoke("FreezeTimeScale", 1f);
        }
    }

    public void initializeRoomScoreboard()
    {
        for (int i = 0; i < 4; i++) // Initializing Scoreboard
        {
            Room.Scoreboard.Add(new KeyValuePair<string, int>("Placeholder", 0));
        }
    }

    private void FreezeTimeScale()
    {
        Debug.Log("Freezing time...");

        //Time.timeScale = 0;
        Room.StartCoroutine(ScaleTime(1.0f, 0.0f, 2.0f));
        //AudioListener.pause = true; // Pause audio playback
        //AudioListener.pause = false; // Restart audio playback --> In StartTimeScale
    }
    IEnumerator ScaleTime(float start, float end, float time)
    {
        float lastTime = Time.realtimeSinceStartup;
        float timer = 0.0f;

        Invoke("setScores", 1f);

        while (timer < time)
        {
            Time.timeScale = Mathf.Lerp(start, end, timer / time);
            AudioSource[] aSources = FindObjectsOfType<AudioSource>();
            foreach (AudioSource aSource in aSources)
                aSource.pitch = Time.timeScale;
            timer += (Time.realtimeSinceStartup - lastTime);
            lastTime = Time.realtimeSinceStartup;
            yield return null;
        }

        Time.timeScale = end;
    }

    private void setScores()
    {
        GameObject scoreboardInstance = Instantiate(Scoreboard_Prefab);
        TextMeshProUGUI[] scoreTexts = scoreboardInstance.GetComponentsInChildren<TextMeshProUGUI>();
        Debug.Log("Number of Score Texts " + scoreTexts.Length);
        for (int i = 1; i < scoreTexts.Length && i <= Room.totalNumOfPlayers; i++)
        {
            scoreTexts[i].text = i + ": " + Room.Scoreboard[i - 1].Key + " " + Room.Scoreboard[i - 1].Value;
        }
        scoreboardInstance.SetActive(true);
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

}
