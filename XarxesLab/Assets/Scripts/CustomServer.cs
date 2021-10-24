using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

enum state
{
    none,
    send,
    await,
    shutDown
}

public class CustomServer : MonoBehaviour
{
    private byte[] data;
    private string input;
    private IPEndPoint ipep;
    private Socket newsocket;
    private Thread waitThread;
    private state serverState;

    // Start is called before the first frame update
    void Start()
    {
        serverState = state.await;

        data = new byte[1024];

        ipep = new IPEndPoint(IPAddress.Any, 9050);

        newsocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        newsocket.Bind(ipep);
    }

    // Update is called once per frame
    void Update()
    {
        switch (serverState)
        {
            case state.none:
                break;
            case state.await:
                waitThread = new Thread(WaitThread);
                waitThread.Start();
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
        newsocket.SendTo(data, data.Length, SocketFlags.None, ipep);
        serverState = state.await;
    }

    void WaitThread()
    {
        Debug.LogWarning("Starting server trhead, waiting for ping!");

        data = new byte[1024];

        try
        {
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)sender;
            int recv = newsocket.ReceiveFrom(data, ref Remote);
            string stringData = Encoding.ASCII.GetString(data, 0, recv);
            Debug.Log("Server: received data is:" + stringData);
            if (stringData == "ping")
            {
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
    }
}
