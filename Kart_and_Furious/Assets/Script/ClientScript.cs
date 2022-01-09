using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Threading;
using System;
using System.IO;
using Complete;

public class ClientScript : MonoBehaviour
{
    private Socket newSocket;
    private IPEndPoint ipep;
    EndPoint server;
    private int recv;
    private Thread helloThread;
    private Thread timeOutThread;
    private float welcomeTimeout = 5.0f;
    private DateTime lastPing;
    private Thread pingThread;
    private Thread msgThread;
    private GameManager gameManager;
    private MemoryStream stream;
    static string localIPAddress = "127.0.0.1";      // TODO: remove when we have input
    private bool isTimeoutTriggered;                // TODO: change for something better
    private bool isDisconnectTriggered;             // TODO: change for something better
    private KartMovement kartScript;
    private int id;
    private bool isPlayerCreated;
    private bool isNewClientCreated;
    private int newClientID;

    private enum ConnectionState
    {
        STATE_DISCONNECTED,
        STATE_HELLO,
        STATE_CONNECTED,
        STATE_DISCONNECTING
    }
    private ConnectionState connectionState = ConnectionState.STATE_DISCONNECTED;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        ipep = new IPEndPoint(IPAddress.Parse(localIPAddress), 2517); // TODO: This needs to be inputed
        server = ipep;
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        isTimeoutTriggered = false;
        isDisconnectTriggered = false;
        isPlayerCreated = false;
        isNewClientCreated = false;
        // TODO: move this when we can input an IP Address
        // {
        byte[] data;// = new byte[1024]; // (?)
        data = Encoding.ASCII.GetBytes("Hello!");
        newSocket.SendTo(data, data.Length, SocketFlags.None, server);

        connectionState = ConnectionState.STATE_HELLO;
        helloThread = new Thread(AwaitWelcome);
        helloThread.Start();
        timeOutThread = new Thread(Timeout);
        timeOutThread.Start();
        // }

    }

    // Update is called once per frame
    void Update()
    {
        switch (connectionState) // Thiis isn't game state! Only state of the connection
        {
            case ConnectionState.STATE_DISCONNECTED:
                {
                    // Socket creation on IPAddress input
                    // Hello! msg sent as soon as the socket is created
                    return;
                }
            case ConnectionState.STATE_HELLO:
                {
                    // Thread awaitng Welcome! msg from server
                    // Timeout timer (could be done with a thread) so it does wait infinitely
                    if (isTimeoutTriggered)
                    {
                        isTimeoutTriggered = false; // Important to try again!
                        helloThread.Abort(); // TODO: Try to avoid
                        Debug.LogWarning("Welcome not received. Timeout expired. Stopping client thread!");
                        connectionState = ConnectionState.STATE_DISCONNECTED;
                    }
                    return;
                }
            case ConnectionState.STATE_CONNECTED:
                {
                    if (!isPlayerCreated)
                    {
                        isPlayerCreated = true;
                        gameManager.AddKart(id);
                    }
                    else if (isNewClientCreated)
                    {
                        isNewClientCreated = false;
                        gameManager.AddKart(newClientID, false);
                    }

                    if (kartScript == null)
                        kartScript = gameObject.GetComponentInChildren<KartMovement>();

                    // Thread awaiting messages from server (Pings!) constantly (different thread for pings (?))
                    DateTime now = System.DateTime.UtcNow;
                    if ((now - lastPing) > TimeSpan.FromSeconds(5))
                        isDisconnectTriggered = true;
                    // Timeout timer for involuntary DC
                    // Active DC option
                    if (isDisconnectTriggered)
                    {
                        isDisconnectTriggered = false;
                        pingThread.Abort();
                        connectionState = ConnectionState.STATE_DISCONNECTING;
                    }
                    return;
                }
            case ConnectionState.STATE_DISCONNECTING:
                {
                    // Shutdown server socket
                    //newSocket.Close(); // (?)
                    connectionState = ConnectionState.STATE_DISCONNECTED;

                    SendMessageToServer("Goodbye!");
                    return;
                }
        }
    }
    void AwaitWelcome()
    {
        Debug.Log("Starting client thread! Awaiting welcome from server...");
        while (connectionState == ConnectionState.STATE_HELLO)
        {
            byte[] data = new byte[1024];

            try
            {
                recv = newSocket.ReceiveFrom(data, ref server);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
                throw e;
            }

            string text = Encoding.ASCII.GetString(data, 0, recv);
            if (text.Contains("Welcome!"))
            {
                connectionState = ConnectionState.STATE_CONNECTED;
                msgThread = new Thread(AwaitMsg);
                msgThread.Start();
                pingThread = new Thread(Ping);
                pingThread.Start();
                lastPing = DateTime.UtcNow;

                string idString = text.Substring(8, 1);
                id = int.Parse(idString); // We get the id
            }
        }
        Debug.Log("Welcome received. Stopping client thread!");
    }

    void AwaitMsg()
    {
        Thread.Sleep(100);
        Debug.Log("Starting client thread! Awaiting messages from server...");
        while (!isDisconnectTriggered)  // TODO: maybe this isn't ideal (?)
        {
            byte[] data = new byte[1024];
            try
            {
                recv = newSocket.ReceiveFrom(data, ref server);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
                throw e;
            }

            string text = Encoding.ASCII.GetString(data, 0, recv);
            if (text == "Ping!")
            {
                lastPing = System.DateTime.UtcNow;
            }
            else if (text == "Disconnect!") // TODO: remove, this was just for testing
            {
                isDisconnectTriggered = true;
            }

            else if (text.Contains("Welcome!"))
            {
                string idString = text.Substring(8, 1);
                int newClientID = int.Parse(idString); // We get the id
                if (id != newClientID)
                {
                    isNewClientCreated = true;
                    this.newClientID = newClientID;
                }
            }
            else if (text.Contains("Key")) // TODO: Probably better to use flags to signalize if a msg is serialized or not
            {
                GetInput(text);
            }
        }
        Debug.Log("Disconnecting from server. Stopping client thread!");
    }

    void Timeout()
    {
        DateTime myTime = System.DateTime.UtcNow;
        while ((System.DateTime.UtcNow - myTime).Seconds < welcomeTimeout)
        {
            if (connectionState != ConnectionState.STATE_HELLO)
            {
                return;
            }
        }
        isTimeoutTriggered = true;
    }

    void Ping()
    {
        Thread.Sleep(100);
        Debug.Log("Starting ping thread!");
        while (true)
        {
            SendMessageToServer("Ping!");
            Thread.Sleep(500);
        }
    }

    public void SendMessageToServer(string message)
    {
        byte[] data;// = new byte[1024]; // (?)
        data = Encoding.ASCII.GetBytes(message + id);
        newSocket.SendTo(data, data.Length, SocketFlags.None, server);
    }

    public string GetLocalIPv4()
    {
        return Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
    }

    void GetInput(string input) // TODO: Separate into 2 funcs: one to extract the inputs and another, off thread, to send them
    {
        string command = input.Remove(input.Length - 1);
        int kartId = int.Parse(input[input.Length - 1].ToString());
        KartMovement kMov = gameManager.kartsList[kartId - 1];
        switch (command)
        {
            case "KeyDownW":
                {
                    kMov.keyW.keyDown = true;
                    return;
                }
            case "KeyUpW":
                {
                    kMov.keyW.keyUp = true;
                    return;
                }
            case "KeyDownS":
                {
                    kMov.keyS.keyDown = true;
                    return;
                }
            case "KeyUpS":
                {
                    kMov.keyS.keyUp = true;
                    return;
                }
            case "KeyDownA":
                {
                    kMov.keyA.keyDown = true;
                    return;
                }
            case "KeyUpA":
                {
                    kMov.keyA.keyUp = true;
                    return;
                }
            case "KeyDownD":
                {
                    kMov.keyD.keyDown = true;
                    return;
                }
            case "KeyUpD":
                {
                    kMov.keyD.keyUp = true;
                    return;
                }
        }
    }

    public void AssignServerIP(string id)
    {

    }

    // TODO: make sure this is needed
    private void OnDestroy()
    {
        if (helloThread != null)
            helloThread.Abort();
        if (timeOutThread != null)
            timeOutThread.Abort();
        if (msgThread != null)
            msgThread.Abort();
        if (pingThread != null)
            pingThread.Abort();
    }
    private void OnDisable() // Debug use
    {
        pingThread.Abort();
    }
}
