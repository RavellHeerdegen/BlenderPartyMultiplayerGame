using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerSuperDashBrothers networkManager;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private TMP_InputField ipAddressInputField = null;
    [SerializeField] private Button joinButton = null;

    private void Start()
    {
        networkManager = (NetworkManagerSuperDashBrothers)FindObjectOfType(typeof(NetworkManagerSuperDashBrothers));
    }

    private void Update()
    {
        if (networkManager == null)
        {
            networkManager = (NetworkManagerSuperDashBrothers)FindObjectOfType(typeof(NetworkManagerSuperDashBrothers));
        }
    }

    private void OnEnable()
    {
        NetworkManagerSuperDashBrothers.OnClientConnected += HandleClientConnected;
        NetworkManagerSuperDashBrothers.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        NetworkManagerSuperDashBrothers.OnClientConnected -= HandleClientConnected;
        NetworkManagerSuperDashBrothers.OnClientDisconnected -= HandleClientDisconnected;
    }

    public void JoinLobby()
    {
        string ipAddress = ipAddressInputField.text;

        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();

        joinButton.interactable = false;
    }

    private void HandleClientConnected()
    {
        joinButton.interactable = true;

        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);
    }

    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
    }
}
