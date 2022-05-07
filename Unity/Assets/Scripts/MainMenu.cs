using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerSuperDashBrothers networkManager = null;

    //public NetworkManagerSuperDashBrothers networkManagerTemplate;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;

    private void Start()
    {
        //if (networkManager == null)
        //{
        //    networkManager = Instantiate(networkManagerTemplate);
        //    networkManager.name = "NetworkManager";
        //}
    }

    public void HostLobby()
    {
        networkManager.StartHost();

        landingPagePanel.SetActive(false);
    }
}
