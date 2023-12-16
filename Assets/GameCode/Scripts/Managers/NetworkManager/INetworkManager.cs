using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkContainer
{
    public interface INetworkManager
    {
        #region Callbacks
        public abstract void I_OnConnectedToMaster();
        public abstract void I_OnJoinedLobby();
        public abstract void I_OnCreatedRoom();
        public abstract void I_OnJoinedRoom();
        public abstract void I_OnJoinRandomFailed();
        public abstract void I_OnLeftRoom();
        public abstract void I_OnPlayerEnteredRoom(PlayerInfo newPlayer);
        public abstract void I_OnPlayerLeftRoom(PlayerInfo player);
        #endregion Callbacks

        #region Functions
        public abstract void ConnectToMaster();
        public abstract void ConnectToLobby();
        public abstract void CreateRoom();
        public abstract void JoinRandom();
        public abstract void JoinRoom(string id);
        public abstract void LoadLevel();
        public abstract void LeaveRoom();
        public abstract void Disconnect();
        #endregion Functions
    }

    public class PlayerInfo
    {
        public int playerID;
        public string playerName;
        public bool   isMasterClient;
    }

    public class RoomInfo
    {
        public string roomName;
        public int maxPlayersInRoom;
        public int playersCount;
        public List<PlayerInfo> playersInfo;
    }
}

