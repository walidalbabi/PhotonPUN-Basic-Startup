using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIMenuManager : MonoBehaviour
{
    public static UIMenuManager instance;

    //Inspector Assign
    [Header("UI Components")]
    [SerializeField] private GameObject _playerNamePanel;
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private GameObject[] _menuPanelChildrens;
    [Header("UI InputFields")]
    [SerializeField] private TMP_InputField _userNameInputField;
    [SerializeField] private TMP_InputField _rooIDInputField;
    [SerializeField] private TMP_InputField _numberOfPlayer;
    [Header("UI Scroll View Contents")]
    [SerializeField] private Transform _roomListingContent;

    [Space]
    [Header("UI Prefabs")]
    [SerializeField] private RoomListingScript _roomListingPrefab;


    //Private
    private Dictionary<string,RoomListingScript> _roolListingList =new Dictionary<string, RoomListingScript>();

    private LoadingScript _loadingScript;
    private string _currentPanelActive;
    private string _previousePanelActive;
    private string _roomIDToConnect;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        if (_loadingScript == null)
        {
            _loadingScript = FindObjectOfType<LoadingScript>();
            _loadingScript.gameObject.SetActive(false);
        }

        //Set Input Fields listener
        _userNameInputField.onValueChanged.AddListener(OnPlayerNameChange);
        _numberOfPlayer.onValueChanged.AddListener(OnMaxPlayersChange);
       // _rooIDInputField.onValueChanged.AddListener(OnIDRoomChange);
    }
    private void Start()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            OnPressConnect();
        }
    }

    #region UI
    /// <summary>
    /// Leave the string variable Empty to disable the loading screen
    /// </summary>
    /// <param name="loadingText"></param>
    public void SetLoading(string loadingText)
    {
        _loadingScript.gameObject.SetActive(true);
        _loadingScript.SetLoading(loadingText);
    }
    public void OnPlayerNameChange(string userName)
    {
        PlayerPrefs.SetString("PlayerName", userName);
    }
    public void OnMaxPlayersChange(string maxPlayers)
    {
        GameNetworkManager.instance.maxPlayers = int.Parse(maxPlayers);
    }
    public void OnIDRoomChange(string id)
    {
        _roomIDToConnect = id;
    }
    public void OnPressConnect()
    {
        _playerNamePanel.SetActive(false);
        GameNetworkManager.instance.ConnectToMaster();
    }
    public void OnPressConnectToRoomDirectly()
    {
      GameNetworkManager.instance.JoinRoom(_roomIDToConnect);
    }
    public void OnPressQuickPlay()
    {
        GameNetworkManager.instance.JoinRandom();
    }
    public void OnPressStartGame()
    {
        GameNetworkManager.instance.LoadLevel();
    }
    public void OnPressLeaveRoom()
    {
        GameNetworkManager.instance.LeaveRoom();
    }

    public void OnPressBack(string panelName)
    {
        if (GetPanelRefferenceByName(panelName) == null) return;

        if (_currentPanelActive != panelName)
            SetPanelActiveByName(panelName);
    }
    #endregion UI

    #region Callbacks
    public void OnConnectedToLobby()
    {
        if (!_menuPanel.activeInHierarchy)
        {
            _menuPanel.SetActive(true);
        }
        SetPanelActiveByName("P_MainMenu");
    }
    public void OnCreateRoom()
    {
        if (_currentPanelActive != "P_Room")
            SetPanelActiveByName("P_Room");
    }
    public void OnJoinedRoom(NetworkContainer.RoomInfo room)
    {
        if (GetPanelRefferenceByName("P_Room") == null) return;

        if (_currentPanelActive != "P_Room")
            SetPanelActiveByName("P_Room");
            GetPanelRefferenceByName("P_Room").GetComponent<RoomPanelScript>().OnRoomEdited(GameNetworkManager.instance.GetLocalPlayerInfo().isMasterClient);


        foreach (var player in room.playersInfo)
        {
            AddPlayerToRoomListing(player);
        }
    }
    public void OnNewPlayerEnterRoom(NetworkContainer.PlayerInfo newPlayer)
    {
        AddPlayerToRoomListing(newPlayer);
        if (GetPanelRefferenceByName("P_Room") != null)
            GetPanelRefferenceByName("P_Room").GetComponent<RoomPanelScript>().OnRoomEdited(GameNetworkManager.instance.GetLocalPlayerInfo().isMasterClient);
    }
    public void OnPlayerLeftRoom(NetworkContainer.PlayerInfo player)
    {
        if(SceneManager.GetActiveScene() == SceneManager.GetSceneAt(0))
        {
            if (_roolListingList.ContainsKey(player.playerName))
            {
                var listing = _roolListingList[player.playerName];
                _roolListingList.Remove(player.playerName);
                if (listing != null)
                    listing.RemoveListing();
            }

            if (player.Equals(GameNetworkManager.instance.GetLocalPlayerInfo()))
            {
                GameNetworkManager.instance.ConnectToLobby();
            }
        }
      
    }
    #endregion Callbacks

    #region Functions
    private void SetPanelActiveByName(string panelName)
    {
        foreach (GameObject panel in _menuPanelChildrens)
        {
            if (panel.name == panelName)
            {
                _previousePanelActive = _currentPanelActive;
                _currentPanelActive = panel.name;
                panel.gameObject.SetActive(true);
            }
            else panel.gameObject.SetActive(false);
        }
    }
    private GameObject GetPanelRefferenceByName(string panelName)
    {
        foreach (GameObject panel in _menuPanelChildrens)
        {
            if (panel.name == panelName)
            {
                return panel;
            }
        }
        return null;
    }
    private void AddPlayerToRoomListing(NetworkContainer.PlayerInfo player)
    {
        if (_roolListingList.ContainsKey(player.playerName)) return;

        RoomListingScript listing = Instantiate(_roomListingPrefab, _roomListingContent);
        listing.SetInfo(player.playerName);
        _roolListingList.Add(player.playerName, listing);
    }
    #endregion Functions
}
