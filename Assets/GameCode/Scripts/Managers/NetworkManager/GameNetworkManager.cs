using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using NetworkContainer;
using System.Linq;

public class GameNetworkManager : MonoBehaviourPunCallbacks, INetworkManager
{
    public static GameNetworkManager instance;

    [SerializeField] private int _maxPlayers = 4;

    public int maxPlayers
    { 
        get{return _maxPlayers;} 
        set { _maxPlayers = value; } 
    }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
    private void OnApplicationQuit()
    {
        Disconnect();
    }

    #region PhotonCallBacks
    public override void OnConnectedToMaster()
    {
        I_OnConnectedToMaster();
    }
    public override void OnJoinedLobby()
    {
        I_OnJoinedLobby();
    }
    public override void OnCreatedRoom()
    {
        I_OnCreatedRoom();
    }
    public override void OnJoinedRoom()
    {
        I_OnJoinedRoom();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        I_OnJoinRandomFailed();

    }
    public override void OnLeftRoom()
    {
        I_OnLeftRoom();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        NetworkContainer.PlayerInfo player = new PlayerInfo()
        {
            playerName = newPlayer.NickName,
        };

        I_OnPlayerEnteredRoom(player);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        NetworkContainer.PlayerInfo player = new PlayerInfo() { 
        playerName = otherPlayer.NickName,
        };

        I_OnPlayerLeftRoom(player);
    }
    #endregion PhotonCallBacks

    #region InterfaceCallbacks
    public  void I_OnConnectedToMaster() 
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log("Connected To Server");
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
        UIMenuManager.instance.SetLoading("Connecing To Lobby...");
    }
    public void I_OnJoinedLobby() 
    {
        Debug.Log("Connected To Lobby");
        base.OnJoinedLobby();
        UIMenuManager.instance.OnConnectedToLobby();
        UIMenuManager.instance.SetLoading("");
    }
    public void I_OnCreatedRoom() 
    {
        Debug.Log($"Room Created Successfuly, MaxPlayers {PhotonNetwork.CurrentRoom.MaxPlayers}");
        base.OnCreatedRoom();
        UIMenuManager.instance.OnCreateRoom();
    }
    public void I_OnJoinedRoom() 
    {
        Debug.Log($"Joined Room {PhotonNetwork.CurrentRoom.Name} Successfuly , MaxPlayers {PhotonNetwork.CurrentRoom.MaxPlayers}");
        base.OnJoinedRoom();
        UIMenuManager.instance.OnJoinedRoom(GetCurrentRoom());
    }
    public void I_OnJoinRandomFailed() 
    {
        //  Debug.LogError("Failed To Join Random Room");
        CreateRoom();
    }
    public void I_OnLeftRoom() 
    {
        Debug.Log("Left Room Successfuly");
        base.OnLeftRoom();

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            UIMenuManager.instance.OnPlayerLeftRoom(GetLocalPlayerInfo());
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
    public void I_OnPlayerEnteredRoom(NetworkContainer.PlayerInfo newPlayer) 
    {
        Debug.Log($"{newPlayer.playerName} New Joined Room Successfuly");
        UIMenuManager.instance.OnNewPlayerEnterRoom(newPlayer);
    }
    public void I_OnPlayerLeftRoom(NetworkContainer.PlayerInfo player) 
    {
        if (player.Equals(GetLocalPlayerInfo()))
            return;
        if (SceneManager.GetActiveScene().buildIndex == 0)
            UIMenuManager.instance.OnPlayerLeftRoom(player);
    }
    #endregion InterfaceCallbacks

    #region InterfaceFunctions
    public void ConnectToMaster()
    {
        string userName = PlayerPrefs.GetString("PlayerName");
        if (!userName.Equals(""))
            PhotonNetwork.LocalPlayer.NickName = userName;

        PhotonNetwork.ConnectUsingSettings();
        UIMenuManager.instance.SetLoading("Connecing To Master...");
    }
    public void ConnectToLobby()
    {
     //   PhotonNetwork.JoinLobby();
    }
    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        roomOptions.MaxPlayers = _maxPlayers;
        PhotonNetwork.CreateRoom(Random.Range(0, 1000).ToString(), roomOptions, TypedLobby.Default);
    }
    public void JoinRandom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        roomOptions.MaxPlayers = _maxPlayers;
        PhotonNetwork.JoinRandomOrCreateRoom(null, 0, MatchmakingMode.RandomMatching, TypedLobby.Default, null, Random.Range(0, 1000).ToString(), roomOptions, null);
    }
    public void JoinRoom(string id)
    {
        PhotonNetwork.JoinRoom(id);
    }
    public void LoadLevel()
    {
        PhotonNetwork.LoadLevel(1);
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    #endregion InterfaceFunctions

    #region Getter
    public NetworkContainer.PlayerInfo GetLocalPlayerInfo()
    {
        NetworkContainer.PlayerInfo playerInfo = new NetworkContainer.PlayerInfo();
        playerInfo.playerID = PhotonNetwork.LocalPlayer.ActorNumber;
        playerInfo.playerName = PhotonNetwork.LocalPlayer.NickName;
        playerInfo.isMasterClient = PhotonNetwork.LocalPlayer.IsMasterClient;

        return playerInfo; 
    }
    public NetworkContainer.RoomInfo GetCurrentRoom() 
    {
        NetworkContainer.RoomInfo roomInfo = new NetworkContainer.RoomInfo();
        roomInfo.roomName = PhotonNetwork.CurrentRoom.Name;
        roomInfo.maxPlayersInRoom = PhotonNetwork.CurrentRoom.MaxPlayers;
        roomInfo.playersCount = PhotonNetwork.CurrentRoom.PlayerCount;
        roomInfo.playersInfo = new List<PlayerInfo>();
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            NetworkContainer.PlayerInfo playerInfo = new NetworkContainer.PlayerInfo();
            playerInfo.playerName = player.Value.NickName;
            playerInfo.isMasterClient = player.Value.IsMasterClient;
            roomInfo.playersInfo.Add(playerInfo);
        }
        return roomInfo;
    }
    public double GetServerTime() { return PhotonNetwork.Time; }
    #endregion Getter

}
