
using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MatchmakingManager : MonoBehaviourPunCallbacks, IMatchmakingCallbacks
{
        private PhotonView localPlayer;
        private LoadBalancingClient _loadBalancingClient;
        private EnterRoomParams roomParams;
        private PhotonView matchmakingPhotonView;

        [Header("Room Parameters")] 
        public string roomName;
        public byte roomMaxPlayers;

        public List<int> playerInQueue;

       /* private void Start()
        { 
                _loadBalancingClient = new LoadBalancingClient();
                matchmakingPhotonView = GetComponent<PhotonView>();
                
                SetRoomParameters();
        }

        private void Update()
        {
                if (playerInQueue.Count >= roomMaxPlayers)
                {
                        CreateGameRoom();
                }
        }

        [PunRPC] public void JoinQueue()
        {
                localPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PhotonView>();
                playerInQueue.Add(localPlayer.ViewID);
        }

        void SetRoomParameters()
        {
                roomParams = new EnterRoomParams();
                roomParams.RoomName = roomName;
                roomParams.RoomOptions.MaxPlayers = roomMaxPlayers; 
        }

        void CreateGameRoom()
        {
                _loadBalancingClient.State = ClientState.ConnectedToMasterServer;
                //_loadBalancingClient.MatchMakingCallbackTargets.Add();
                _loadBalancingClient.OpJoinOrCreateRoom(roomParams);
        }
        */
}