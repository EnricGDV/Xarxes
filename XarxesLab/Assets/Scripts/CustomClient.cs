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

public class CustomClient : MonoBehaviour
{
    private byte[] data;
    private Thread waitThread;
    private IPEndPoint ipep;
    private Socket server;
    private state clientState;
    private int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        clientState = state.send;

        data = new byte[1024];

        ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);

        server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

    }


    // Update is called once per frame
    void Update()
    {
        if (count >= 5)
            clientState = state.shutDown;

        switch (clientState)
        {
            case state.none:
                break;
            case state.send:
                Debug.Log("Client: sending ping");
                SendPing();
                break;
            case state.await:
                waitThread = new Thread(WaitThread);
                waitThread.Start();
                break;
            case state.shutDown:
                if (waitThread.IsAlive)
                    Debug.Log("Thread is Alive! Can't Shut Down!!!");
                Debug.Log("Stopping server from CustomClient.cs");
                server.Close();
                break;
        }

    }


    void SendPing()
    {
        data = Encoding.ASCII.GetBytes("ping");
        server.SendTo(data, data.Length, SocketFlags.None, ipep);
        clientState = state.await;
    }

    void WaitThread()
    {
        Debug.LogWarning("Starting client trhead, waiting for pong!");

        data = new byte[1024];

        try
        {
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)sender;
            int recv = server.ReceiveFrom(data, ref Remote);
            string stringData = Encoding.ASCII.GetString(data, 0, recv);
            Debug.Log("Client: received data is:" + stringData);
            if (stringData == "pong")
            {
                Thread.Sleep(500);
                clientState = state.send;
                count++;
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("Client: Connection failed.. trying again...");
            Debug.Log(e);
        }
        Debug.Log("Stopping client, pong received!");
    }
}
