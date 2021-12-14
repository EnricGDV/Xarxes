using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Threading;

public class ClientScript : MonoBehaviour
{
    private Socket newSocket;
    private byte[] data;
    private IPEndPoint ipep;
    EndPoint server;
    private int recv;
    private string text;
    private Thread helloThread;
    private Thread msgThread;
    static string ipAddress204 = "192.168.204.24"; //TODO: remove when we have input
    static string localIPAddress = "127.0.0.1";    //TODO: remove when we have input
    private bool isTimeoutTriggered = false; // TODO: change for something better
    private enum ConnectionState
    {
        STATE_NONE,
        STATE_HELLO,
        STATE_CONNECTED,
        STATE_DISCONNECTING
    }
    private ConnectionState connectionState = ConnectionState.STATE_NONE;

    // Start is called before the first frame update
    void Start()
    {
        ipep = new IPEndPoint(IPAddress.Parse(localIPAddress), 2517); // TODO: This needs to be inputed
        server = ipep;
        data = new byte[1024];
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // TODO: move this when we can input an IP Address
        // {
        data = Encoding.ASCII.GetBytes("Hello!");
        newSocket.SendTo(data, data.Length, SocketFlags.None, ipep);
        connectionState = ConnectionState.STATE_HELLO;
        helloThread = new Thread(AwaitWelcome);
        helloThread.Start();
        // }
    }

    // Update is called once per frame
    void Update()
    {
        switch (connectionState)
        {
            case ConnectionState.STATE_NONE:
                {
                    // Socket creation on IPAddress input
                    // Hello! msg sent as soon as the socket is created
                    return;
                }
            case ConnectionState.STATE_HELLO:
                {
                    // Thread awaitng Welcome! msg from server
                    // Timeout timer (could be done with a thread) so it does wait infinitely
                    return;
                }
            case ConnectionState.STATE_CONNECTED:
                {
                    // Thread awaiting messages from server (Pings!) constantly (different thread for pings (?))
                    // Timeout timer for involuntary DC
                    // Active DC option
                    return;
                }
            case ConnectionState.STATE_DISCONNECTING:
                {
                    // Shutdown server socket
                    return;
                }
        }
    }
    void AwaitWelcome()
    {
        Debug.Log("Starting client thread! Awaiting welcome from server...");
        data = new byte[1024];// TODO: I think I use this where and when I'm not supposed to

        while (connectionState == ConnectionState.STATE_HELLO && !isTimeoutTriggered) // This loop can be broken from outside the thread through a timeout handled elsewhere
        {
            try
            {
                recv = newSocket.ReceiveFrom(data, ref server);
                text = Encoding.ASCII.GetString(data, 0, recv);

                if (text == "Welcome!")
                {
                    connectionState = ConnectionState.STATE_CONNECTED;
                    msgThread = new Thread(AwaitMsg);
                    msgThread.Start();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
                throw e;
            }
        }
        if (isTimeoutTriggered)
        {
            Debug.Log("Welcome not received. Timeout expired. Stopping client thread!");
        }
        else
            Debug.Log("Welcome received. Stopping client thread!");
    }

    void AwaitMsg()
    {
        Debug.Log("Starting client thread! Awaiting messages from server...");
        data = new byte[1024];
        while (connectionState == ConnectionState.STATE_CONNECTED) // TODO: maybe this isn't ideal (?)
        {
            try
            {
                recv = newSocket.ReceiveFrom(data, ref server);
                text = Encoding.ASCII.GetString(data, 0, recv);
                Debug.Log(text);
                if (text == "Disconnect!") 
                {
                    connectionState = ConnectionState.STATE_DISCONNECTING;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
                throw e;
            }
        }
        Debug.Log("Disconnecting from server. Stopping client thread!");
    }
    public string GetLocalIPv4()
    {
        return Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
    }

    // TODO: make sure this isn't needed
    //private void OnDestroy() 
    //{
    //    helloThread.Abort();
    //    msgThread.Abort();
    //}
}
