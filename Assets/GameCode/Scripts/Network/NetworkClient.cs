using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkClient : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GameObject _playerPrefab;

    private int _playerID;

    private PhotonView _photonView;
    private Transform _spawmPoint;
    private GameObject _possessedEntity;

    public int playerID { get { return _playerID; } }

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        _playerID = _photonView.ViewID;
    }

    private void OnDestroy()
    {
    }

    #region PlayerInitialization
    private void InitPlayer()
    {
        _photonView.RPC("RPC_GetSpawnPoint", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_GetSpawnPoint()
    {
       // _spawmPoint = NGameManager.instance.GetNextSpawnPoint();

        if (_photonView.IsMine)
        {
            _photonView.RPC("RPC_SpawnPlayer", RpcTarget.AllBuffered, _photonView.ViewID, GameNetworkManager.instance.GetLocalPlayerInfo().playerName);
        } 
    }
    [PunRPC]
    private void RPC_SpawnPlayer(int ownerID, string username)
    {
        _possessedEntity = Instantiate(_playerPrefab, _spawmPoint.position, Quaternion.identity);

    }
    #endregion PlayerInitialization

    #region Callbacks

    #endregion Callbacks

}
