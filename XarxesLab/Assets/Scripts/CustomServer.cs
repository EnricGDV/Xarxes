using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;



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
        Debug.Log("Stopping server, ping received!");
        waitThreadCreated = false;
    }
}
