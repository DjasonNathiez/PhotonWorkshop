using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerNetworkSetup : MonoBehaviour
{
    
    private NetworkManager _networkManager;
    public PlayerController PlayerController;
    public GameObject PlayerCamera;
    
    [Header("Interface")]
    public GameObject HubInterface;
    public GameObject InGameInterface;
    public TextMeshProUGUI roomNameTxt;
    public TextMeshProUGUI healthInfoText;
    public TextMeshProUGUI textConnect;
    public GameObject WorldInfoInterface;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        gameObject.name = "Player_" + GetComponent<PhotonView>().ViewID;
        _networkManager = FindObjectOfType<NetworkManager>();
    }

    public void InitPlayer() //Initialize les informations joueurs et sépare bien les clients. Appelé à l'instantiation du client.
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            PlayerCamera.SetActive(true);
            PlayerController.enabled = true;
            HubInterface.SetActive(true);
            InGameInterface.SetActive(true);
            //WorldInfoInterface.SetActive(true);
        }
        else
        {
            PlayerCamera.SetActive(false);
            PlayerController.enabled = false;
            HubInterface.SetActive(false);
            InGameInterface.SetActive(false);
            //WorldInfoInterface.SetActive(false);
        }
    }
    
    public void JoinQueue() //Utilisation d'une fonction local qui appelle une fonction RPC pour effectuer la connection réseau.
    {
        _networkManager.JoinQueue();
    }
    
    public void DisableInterface()
    {
        HubInterface.SetActive(false);
        InGameInterface.SetActive(true);
    }
}
