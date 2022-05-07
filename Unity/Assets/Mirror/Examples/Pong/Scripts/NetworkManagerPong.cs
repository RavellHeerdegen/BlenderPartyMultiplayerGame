using Mirror;
using UnityEngine;


    // Custom NetworkManager that simply assigns the correct racket positions when
    // spawning players. The built in RoundRobin spawn method wouldn't work after
    // someone reconnects (both players would be on the same side).
    [AddComponentMenu("")]
    public class NetworkManagerPong : NetworkManager
    {

        GameObject timer;
        public Transform playerOneSpawn;
        public Transform playerTwoSpawn;
        public Transform playerThreeSpawn;
        public Transform playerFourSpawn;

        public GameObject gameflowController;

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            // add player at correct spawn position
            Transform start = null;
            GameObject player = null;
            switch(numPlayers)
            {
                case 0:
                    start = playerOneSpawn;
                    player = Instantiate(playerPrefab, start.position, start.rotation);
                    //gameflowController.GetComponent<GameControlling>().player_One = player;
                    break;
                case 1:
                    start = playerTwoSpawn;
                    player = Instantiate(playerPrefab, start.position, start.rotation);
                    //gameflowController.GetComponent<GameControlling>().player_Two = player;
                    break;
                case 2:
                    start = playerThreeSpawn;
                    player = Instantiate(playerPrefab, start.position, start.rotation);
                    //gameflowController.GetComponent<GameControlling>().player_Two = player;
                    break;
                case 3:
                    start = playerFourSpawn;
                    player = Instantiate(playerPrefab, start.position, start.rotation);
                    //gameflowController.GetComponent<GameControlling>().player_Two = player;
                    break;
        }
            NetworkServer.AddPlayerForConnection(conn, player);



            // spawn timer if two players
            if (numPlayers >= 2)
            {

                timer = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Timer"));

                NetworkServer.Spawn(timer);
            }
    }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            // destroy ball
            if (timer != null)
                NetworkServer.Destroy(timer);

            // call base functionality (actually destroys the player)
            base.OnServerDisconnect(conn);
        }
    }

