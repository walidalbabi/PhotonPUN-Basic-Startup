using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingScript : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _loadingTxt;

    /// <summary>
    /// Leave the string variable Empty to disable the loading screen
    /// </summary>
    /// <param name="loadingText"></param>
    public void SetLoading(string loadingText)
    {
        if (loadingText.Equals(string.Empty))
        {
            gameObject.SetActive(false);
            return;
        }

        _loadingTxt.text = loadingText;
    }
}
