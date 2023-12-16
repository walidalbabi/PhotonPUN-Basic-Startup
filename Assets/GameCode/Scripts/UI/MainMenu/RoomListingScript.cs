using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomListingScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerName;

    public void SetInfo(string playerName)
    {
        _playerName.text = playerName;
    }
    public void RemoveListing()
    {
        Destroy(gameObject);
    }
}
