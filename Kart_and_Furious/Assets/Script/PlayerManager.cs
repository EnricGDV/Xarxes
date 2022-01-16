using UnityEngine;
using UnityEngine.EventSystems;

using Photon.Pun;

using System.Collections;


public class PlayerManager : MonoBehaviourPunCallbacks
{
    #region Private Fields

    #endregion

    #region Public Fields

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    #endregion


    #region MonoBehaviour CallBacks

    void Awake()
    {
        // Used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            PlayerManager.LocalPlayerInstance = this.gameObject;
            this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
            this.gameObject.transform.eulerAngles = new Vector3(this.gameObject.transform.rotation.eulerAngles.x, -90f, this.gameObject.transform.rotation.eulerAngles.z);
        }

        // We flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
    }


    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    void Start()
    {
        CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();


        if (_cameraWork != null)
        {
            if (photonView.IsMine)
            {
                _cameraWork.OnStartFollowing();
            }
        }
        else
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
        }
    }

    #endregion
}
