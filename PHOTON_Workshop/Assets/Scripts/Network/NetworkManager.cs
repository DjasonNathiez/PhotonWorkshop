using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Debug")]
    public PlayerNetworkSetup _playerNetworkSetup;
    public TextMeshProUGUI textToConnection;
    public GameObject cameraLoad;
    
    private PhotonView networkPhotonView;

    [Space] public GameObject playerPrefab;
    public List<string> connectedPlayers;

    [Header("Lobby Connexion")]
    private TypedLobby gameLobby = new TypedLobby("gameLobby", LobbyType.Default);
    private TypedLobby hubLobby = new TypedLobby("hubLobby", LobbyType.Default);
    public LobbyToConnect lobbyToConnect;
    public enum LobbyToConnect { HUB, GAME }

    public PhotonView localPlayer;
    
    [Header("Matchmaking")] 
    public bool localInQueue;
    public List<int> playerInQueue;
    public int numberToLoad;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        networkPhotonView = GetComponent<PhotonView>();
        Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
    }

    private void Start()
    {
        textToConnection.text = "Connecting...";
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Update()
    {
        if (localInQueue)
        {
            _playerNetworkSetup.textConnect.text = "In Queue... " + playerInQueue.Count + "/" + numberToLoad;
        }

        if (PhotonNetwork.CurrentLobby == gameLobby)
        {
            if (!_playerNetworkSetup.InGameInterface.activeSelf)
            {
                _playerNetworkSetup.InGameInterface.SetActive(true);
                
            }
            else
            {
                if (_playerNetworkSetup.roomNameTxt.text != PhotonNetwork.CurrentRoom.Name)
                {
                    _playerNetworkSetup.roomNameTxt.text = PhotonNetwork.CurrentRoom.Name;
                }
            }
        }
    }

    #region SERVER MANAGEMENT

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        textToConnection.text = "Connected to server.";

        switch (lobbyToConnect)
        {
            case LobbyToConnect.HUB:
                PhotonNetwork.JoinLobby(hubLobby);
                break;
            
            case LobbyToConnect.GAME:
                PhotonNetwork.JoinLobby(gameLobby);
                break;
        }
        
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        textToConnection.text = "You've joined the lobby " + PhotonNetwork.CurrentLobby.Name;
        
        if (PhotonNetwork.CurrentLobby == gameLobby)
        {
            string newRoom = "gameroom_" + PhotonNetwork.CountOfRooms;
            PhotonNetwork.JoinOrCreateRoom(newRoom, null, gameLobby);
        }
        
        if(PhotonNetwork.CurrentLobby == hubLobby)
        {
            PhotonNetwork.JoinOrCreateRoom("Hub", null, null);
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        textToConnection.text = "You've joined the room : " + PhotonNetwork.CurrentRoom.Name;
        
        if (!localPlayer)
        {
            InstantiatePlayer(new Vector3(0,1,0));
        }
        
        if (PhotonNetwork.CurrentLobby == gameLobby)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }

    #endregion

    #region INITIALISATION
    public void InstantiatePlayer(Vector3 spawnPosition)
    {
        GameObject newPlayer = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);

        localPlayer = newPlayer.GetComponent<PhotonView>();
        
        _playerNetworkSetup = newPlayer.GetComponent<PlayerNetworkSetup>();
        
        _playerNetworkSetup.InitPlayer();

        _playerNetworkSetup.textConnect.text = textToConnection.text;
        _playerNetworkSetup.InGameInterface.SetActive(false);

        textToConnection.transform.parent.gameObject.SetActive(false);
        cameraLoad.SetActive(false);
    }

    #endregion

    #region CONNECT

    //Lobby
    public void JoinGameLobby()
    {
        _playerNetworkSetup.textConnect.text = "Connecting to the game room...";
        PhotonNetwork.LeaveRoom();
        lobbyToConnect = LobbyToConnect.GAME;
        
        networkPhotonView.RPC("RemoveFromList", RpcTarget.AllBufferedViaServer, localPlayer.ViewID);
    }

    public void JoinHubLobby()
    {
        PhotonNetwork.LeaveRoom();
        lobbyToConnect = LobbyToConnect.HUB;
    }

    public void JoinQueue()
    {
        if (!localInQueue)
        {
            networkPhotonView.RPC("AddPlayerToQueue", RpcTarget.AllBufferedViaServer, localPlayer.ViewID);
            localInQueue = true;
        }
    }

    [PunRPC] public void AddPlayerToQueue(int id)
    {
        playerInQueue.Add(id);
        UpdateQueueState(id);
    }

    public void UpdateQueueState(int localPlayerId)
    {
        foreach (var queueID in playerInQueue)
        {
                if (queueID == localPlayerId && localInQueue)
                {
                    if (playerInQueue.Count >= numberToLoad)
                    {
                        JoinGameLobby();
                    } 
                }
        }
    }

    [PunRPC]
    public void RemoveFromList(int id)
    {
        if (playerInQueue.Contains(id))
        {
            playerInQueue.Remove(id);
        }
    }

    #endregion
}
