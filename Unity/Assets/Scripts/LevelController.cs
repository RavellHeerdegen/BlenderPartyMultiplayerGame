using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using UnityEngine;

public class LevelController : NetworkBehaviour
{
    //public Camera maincamera;

    bool createdLevel = false;

    private GameObject levelTwoPrefab = null;
    private GameObject levelThreePrefab = null;


    // Start is called before the first frame update
    void Start()
    {
        levelTwoPrefab = (GameObject)Resources.Load("SpawnablePrefabs/LevelTwo");
        levelThreePrefab = (GameObject)Resources.Load("SpawnablePrefabs/LevelThree");
        /*
        if (maincamera == null)
        {
            try
            {
                //maincamera = GameObject.Find("Main Camera").GetComponent<Camera>();

            }
            catch(System.Exception e)
            {
                Debug.Log(e);
            }
            
        }*/
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
        Debug.Log("!!!!!!!!");
        if (other.gameObject.name == "LevelCeiling")
        {
            //gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(Room.GamePlayers[0].GetComponent<NetworkIdentity>().connectionToClient);
            CreateNewLevel();
        } else if (other.gameObject.name == "Rotorblade")
        {
            Debug.Log("ahhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh!!");
            NetworkServer.Destroy(gameObject.transform.parent.transform.parent.gameObject);
        }
    }

    /*private void FixedUpdate()
    {
        if (maincamera == null)
        {
            try
            {
                maincamera = GameObject.Find("Main Camera").GetComponent<Camera>();
                if (maincamera != null)
                {
                    Debug.Log("Maincamera im Levelcontroller gefunden");
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }
        }
        else
        {
            if (!createdLevel)
            {
                Vector3 screenPoint = maincamera.WorldToViewportPoint(gameObject.transform.position);

                bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
                Debug.Log(onScreen + " " +  screenPoint);
                if (onScreen)
                {
                    if (!isServer) { return; }
                    Debug.Log("CEILING IS IN SIGHT!!!");
                    createdLevel = true;
                    //gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(Room.GamePlayers[0].GetComponent<NetworkIdentity>().connectionToClient);
                    RpcCreateNewLevel();
                }
            }
        }
    }*/

    void CreateNewLevel()
    {
        Debug.Log("Creating a new level");
        //RpcCreateNewLevelOnClient();

        GameObject newlevel = null;
        if (Room.levelToggle) {
            Debug.Log("Spawning Level Three");
            newlevel = Instantiate(levelThreePrefab);
            Room.levelToggle = false;
        } else {
            Debug.Log("Spawning Level Two");
            newlevel = Instantiate(levelTwoPrefab);
            Room.levelToggle = true;
        }

        GameObject spawnpointLevel = GameObject.Find("SpawnPointLevel") as GameObject;

        newlevel.transform.position = spawnpointLevel.transform.position;

        //NetworkServer.Spawn(newlevel);
    }

    [ClientRpc]
    void RpcCreateNewLevelOnClient()
    {
        //GameObject newlevel = Instantiate(gameflowcontroller.GetComponent<GameControlling>().levels[levelindex]) as GameObject;
        
        //newlevel.transform.position = new Vector3(startinglevel.transform.position.x,
        //    referenceObject.gameObject.transform.position.y + 10f, startinglevel.transform.position.z);

        //// Set gameflowcontroller for all clients
        //newlevel.GetComponentInChildren<LevelController>().gameflowcontroller = gameflowcontroller;

        //NetworkServer.Spawn(newlevel); // Spawn new level on all clients
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
