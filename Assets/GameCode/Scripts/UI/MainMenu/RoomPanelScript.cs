using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPanelScript : MonoBehaviour
{
    [SerializeField] private GameObject _startButton;

    public void OnRoomEdited(bool isHost)
    {
        if (isHost) _startButton.gameObject.SetActive(true);
        else _startButton.gameObject.SetActive(false);
    }
}
