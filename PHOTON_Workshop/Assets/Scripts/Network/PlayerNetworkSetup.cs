using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerNetworkSetup : MonoBehaviour
{
    public TextMeshProUGUI textConnect;
    private NetworkManager _networkManager;
    public PlayerController PlayerController;
    public GameObject PlayerCamera;
    
    [Header("Interface")]
    public GameObject HubInterface;
    public GameObject InGameInterface;
    public TextMeshProUGUI roomNameTxt;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        gameObject.name = "Player_" + GetComponent<PhotonView>().ViewID;
        _networkManager = FindObjectOfType<NetworkManager>();
    }

    public void InitPlayer()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            PlayerCamera.SetActive(true);
            PlayerController.enabled = true;
            HubInterface.SetActive(true);
            InGameInterface.SetActive(true);
        }
        else
        {
            PlayerCamera.SetActive(false);
            PlayerController.enabled = false;
            HubInterface.SetActive(false);
            InGameInterface.SetActive(false);
        }
    }
    
    public void JoinQueue()
    {
        _networkManager.JoinQueue();
    }
    
    public void DisableInterface()
    {
        HubInterface.SetActive(false);
        InGameInterface.SetActive(true);
    }
}
