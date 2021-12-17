using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerIPField : MonoBehaviour
{
    public GameObject serverIPtext;
    public GameObject client;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ServerIPInput()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            client.GetComponent<ClientScript>().AssignServerIP(serverIPtext.GetComponent<Text>().text);
        }
    }
}
