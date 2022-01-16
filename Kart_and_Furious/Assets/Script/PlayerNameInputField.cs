using UnityEngine;
using UnityEngine.UI;


using Photon.Pun;
using Photon.Realtime;


using System.Collections;



    /// Player name input field. Let the user input his name, will appear above the player in the game.
    [RequireComponent(typeof(InputField))]
public class PlayerNameInputField : MonoBehaviour
{
    #region Private Constants


    // Store the PlayerPref Key to avoid typos
    const string playerNamePrefKey = "PlayerName";


    #endregion


    #region MonoBehaviour CallBacks


    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    void Start()
    {


        string defaultName = string.Empty;
        InputField _inputField = this.GetComponent<InputField>();
        if (_inputField != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                _inputField.text = defaultName;
            }
        }


        PhotonNetwork.NickName = defaultName;
    }


    #endregion


    #region Public Methods


    /// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
    /// <param name="value">The name of the Player</param>
    public void SetPlayerName(string value)
    {

        if (string.IsNullOrEmpty(value))
        {
            Debug.LogWarning("Player Name is null or empty");
            return;
        }
        PhotonNetwork.NickName = value;


        PlayerPrefs.SetString(playerNamePrefKey, value);
    }


    #endregion
}