#define TCP_B

using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

//################################################################################      UDP SERVER       ###########################################################################
#if UDP

public class CustomServer : MonoBehaviour
{

    enum state
    {
        none,
        send,
        await,
        shutDown
    }

    private byte[] data;
    private string input;
    private IPEndPoint ipep;
    private Socket newsocket;
    private Thread waitThread;
    private state serverState;
    private IPEndPoint sender;
    private EndPoint Remote;
    private bool waitThreadCreated = false;
    // Start is called before the first frame update
    void Start()
    {
        serverState = state.await;

        data = new byte[1024];

        ipep = new IPEndPoint(IPAddress.Any, 9050);

        newsocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        newsocket.Bind(ipep);


        sender = new IPEndPoint(IPAddress.Any, 0);
        Remote = (EndPoint)sender;
    }

    // Update is called once per frame
    void Update()
    {
        switch (serverState)
        {
            case state.none:
                break;
            case state.await:
                if (!waitThreadCreated)
                {
                    waitThread = new Thread(WaitThread);
                    waitThread.Start();
                    waitThreadCreated = true;
                }
                else if (waitThreadCreated)
                {
                    // Debug.LogWarning("Server: thread is living la vida loca");
                }

                break;
            case state.send:
                SendPong();
                break;
            case state.shutDown:
                if (waitThread.IsAlive)
                    Debug.Log("Thread is Alive! Can't Shut Down!!!");
                serverState = state.none;
                break;
        }
    }

    void SendPong()
    {
        data = Encoding.ASCII.GetBytes("pong");
        newsocket.SendTo(data, data.Length, SocketFlags.None, Remote);
        serverState = state.await;
    }

    void WaitThread()
    {
        //Debug.LogWarning("Starting server trhead, waiting for ping!");

        data = new byte[1024];

        try
        {
            int recv = newsocket.ReceiveFrom(data, ref Remote);
            string stringData = Encoding.ASCII.GetString(data, 0, recv);
            //Debug.Log("Server: received data is:" + stringData);
            if (stringData == "ping")
            {
                Debug.Log("Server: client sent PING");
                Thread.Sleep(500);
                serverState = state.send;
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("Server: Connection failed.. trying again...");
            Debug.Log(e);
        }

        waitThreadCreated = false;
    }
}
#endif


//################################################################################      TCP SERVER A      ###########################################################################

#if TCP_A

public class CustomServer : MonoBehaviour
{

    enum stateTCP
    {
        none,
        listen,
        send,
        await,
        shutDown
    }

    private byte[] data;
    private string input;
    private IPEndPoint ipep;
    private Socket newsocket;
    private Socket client;
    private Thread waitThread;
    private Thread listenThread;
    private stateTCP serverState;
    private IPEndPoint clientep;  //AKA sender;

    private bool waitThreadCreated = false;
    private bool listenThreadCreated = false;
    // Start is called before the first frame update
    void Start()
    {
        serverState = stateTCP.listen;

        data = new byte[1024];

        ipep = new IPEndPoint(IPAddress.Any, 9050);

        newsocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        newsocket.Bind(ipep);


    }

    // Update is called once per frame
    void Update()
    {
        switch (serverState)
        {
            case stateTCP.none:
                break;
            case stateTCP.listen:

                if (!listenThreadCreated)
                {
                    listenThread = new Thread(ListenThread);
                    listenThread.Start();
                    listenThreadCreated = true;
                }
                break;
            case stateTCP.await:
                if (!waitThreadCreated)
                {
                    waitThread = new Thread(WaitThread);
                    waitThread.Start();
                    waitThreadCreated = true;
                }
                else if (waitThreadCreated)
                {
                    // Debug.LogWarning("Server: thread is living la vida loca");
                }

                break;
            case stateTCP.send:
                SendPong();
                break;
            case stateTCP.shutDown:
                if (waitThread.IsAlive)
                    Debug.Log("Thread is Alive! Can't Shut Down!!!");
                client.Close();
                newsocket.Close();
                serverState = stateTCP.none;
                
                break;
        }
    }

    void SendPong()
    {
        data = Encoding.ASCII.GetBytes("pong");
        client.Send(data, SocketFlags.None);
        
        serverState = stateTCP.await;
    }

    void WaitThread()
    {
        //Debug.LogWarning("Starting server trhead, waiting for ping!");

        data = new byte[1024];

        try
        {
            int recv = client.Receive(data);
            string stringData = Encoding.ASCII.GetString(data, 0, recv);
            //Debug.Log("Server: received data is:" + stringData);
            if (stringData == "ping")
            {
                Debug.Log("Server: client sent PING");
                Thread.Sleep(500);
                serverState = stateTCP.send;
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("Server: Connection failed.. trying again...");
            Debug.Log(e);
        }

        waitThreadCreated = false;
    }

    void ListenThread()
    {

        try
        {
            Debug.Log("Server: Starting listening thread, Waiting for a client...");
            newsocket.Listen(10);
            Thread.Sleep(500);
            client = newsocket.Accept();
            clientep = (IPEndPoint)client.RemoteEndPoint;     
        }
        catch (System.Exception e)
        {
            Debug.Log("Server: listening failed.. client not connected trying again...");
            Debug.Log(e);
        }
        serverState = stateTCP.await;
    }

}
#endif

#if TCP_B

public class CustomServer : MonoBehaviour
{

    enum stateTCP
    {
        none,
        listen,
        send,
        await,
        shutDown
    }

    private byte[] data;
    private string input;
    private IPEndPoint ipep;
    private Socket newsocket;
    private Socket client;
    private Thread waitThread;
    private Thread listenThread;
    private stateTCP serverState;
    private IPEndPoint clientep;  //AKA sender;

    private bool waitThreadCreated = false;
    private bool listenThreadCreated = false;
    // Start is called before the first frame update
    void Start()
    {
        serverState = stateTCP.listen;

        data = new byte[1024];

        ipep = new IPEndPoint(IPAddress.Any, 9050);

        newsocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        newsocket.Bind(ipep);


    }

    // Update is called once per frame
    void Update()
    {
        switch (serverState)
        {
            case stateTCP.none:
                break;
            case stateTCP.listen:

                if (!listenThreadCreated)
                {
                    listenThread = new Thread(ListenThread);
                    listenThread.Start();
                    listenThreadCreated = true;
                }
                break;
            case stateTCP.await:
                if (!waitThreadCreated)
                {
                    waitThread = new Thread(WaitThread);
                    waitThread.Start();
                    waitThreadCreated = true;
                }
                else if (waitThreadCreated)
                {
                    // Debug.LogWarning("Server: thread is living la vida loca");
                }

                break;
            case stateTCP.send:
                SendPong();
                break;
            case stateTCP.shutDown:
                if (waitThread.IsAlive)
                    Debug.Log("Thread is Alive! Can't Shut Down!!!");
                client.Close();
                newsocket.Close();
                serverState = stateTCP.none;

                break;
        }
    }

    void SendPong()
    {
        data = Encoding.ASCII.GetBytes("pong");
        client.Send(data, SocketFlags.None);

        serverState = stateTCP.await;
    }

    void WaitThread()
    {
        //Debug.LogWarning("Starting server trhead, waiting for ping!");

        data = new byte[1024];

        try
        {
            int recv = client.Receive(data);
            string stringData = Encoding.ASCII.GetString(data, 0, recv);
            //Debug.Log("Server: received data is:" + stringData);
            if (stringData == "ping")
            {
                Debug.Log("Server: client sent PING");
                Thread.Sleep(500);
                serverState = stateTCP.send;
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("Server: Connection failed.. trying again...");
            Debug.Log(e);
        }

        waitThreadCreated = false;
    }

    void ListenThread()
    {

        try
        {
            Debug.Log("Server: Starting listening thread, Waiting for a client...");
            newsocket.Listen(10);
            Thread.Sleep(500);
            client = newsocket.Accept();
            clientep = (IPEndPoint)client.RemoteEndPoint;
        }
        catch (System.Exception e)
        {
            Debug.Log("Server: Trying to look for clients");
            Debug.Log(e);
        }
        serverState = stateTCP.await;
    }

}
#endif

//################################################################################      TCP SERVER B      ###########################################################################

