using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpawnSystem : NetworkBehaviour
{
    [SerializeField] private GameObject[] playerPrefabs = null;

    private static List<Transform> spawnPoints = new List<Transform>();

    [SyncVar]
    private int nextIndex = 0;

    [SyncVar]
    public int playerCount = 0;

    public static void AddSpawnPoint(Transform transform)
    {
        spawnPoints.Add(transform);

        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }
    public static void RemoveSpawnPoint(Transform transform) => spawnPoints.Remove(transform);

    public override void OnStartServer() => NetworkManagerSuperDashBrothers.OnServerReadied += SpawnPlayer;

    //public override void OnStartClient()
    //{
    //    InputManager.Add(ActionMapNames.Player);
    //    InputManager.Controls.Player.Look.Enable();
    //}

    [ServerCallback]
    private void OnDestroy() => NetworkManagerSuperDashBrothers.OnServerReadied -= SpawnPlayer;

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);

        if (spawnPoint == null)
        {
            Debug.LogError($"Missing spawn point for player {nextIndex}");
            return;
        }

        GameObject playerInstance = Instantiate(playerPrefabs[nextIndex], spawnPoints[nextIndex].position, new Quaternion(0,0,0,0));
        playerInstance.GetComponent<CollisionDetectorPlayer>().networkGamePlayerName = Room.Scoreboard[nextIndex].Key;

        //float x = playerInstance.GetComponent<BoxCollider2D>().size.x;
        //float y = playerInstance.GetComponent<BoxCollider2D>().size.y;

        //playerInstance.GetComponentInChildren<SpriteRenderer>().sprite = sprites[4];
        //playerInstance.transform.localScale = new Vector3(10, 10, 0);

        //playerInstance.GetComponent<BoxCollider2D>().size = new Vector3(x, y, 0);
        NetworkServer.Spawn(playerInstance, conn);

        nextIndex++;
        playerCount++;
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
