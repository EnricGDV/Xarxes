using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CustomClient : MonoBehaviour
{
    enum stateTCP
    {
        none,
        connect,
        send,
        await,
        shutDown
    }

    struct message
    {
        string name;
        string text;
    }

    private byte[] data;
    private Thread connectThread;
    private Thread waitThread;
    private IPEndPoint ipep;
    private Socket server;
    private stateTCP clientState;
    private int count;
    private bool waitThreadCreated;
    private bool connectThreadCreated;
    private message[] messages;

    public GameObject chat;
    public GameObject receivedmsg;
    public GameObject sentmsg;

    // Start is called before the first frame update
    void Start()
    {
        clientState = stateTCP.connect;

        data = new byte[1024];

        ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);

        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        waitThreadCreated = false;
        connectThreadCreated = false;

        count = 0;
    }


    // Update is called once per frame
    void Update()
    {
        if (count >= 5 && clientState != stateTCP.none)
            clientState = stateTCP.shutDown;

        switch (clientState)
        {
            case stateTCP.none:
                break;
            case stateTCP.connect:
                if (!connectThreadCreated)
                {
                    connectThread = new Thread(ConnectThread);
                    connectThread.Start();
                }

                break;
            case stateTCP.send:
                SendPing();
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
                    //Debug.LogWarning("Client: thread is living la vida loca");
                }
                break;
            case stateTCP.shutDown:
                if (waitThread.IsAlive)
                    Debug.Log("Thread is Alive! Can't Shut Down!!!");
                Debug.Log("Stopping server from CustomClient.cs");
                clientState = stateTCP.none;
                server.Close();
                break;
        }

    }


    void SendPing()
    {
        server.Send(Encoding.ASCII.GetBytes("ping"));
        clientState = stateTCP.await;
    }

    void WaitThread()
    {
        //Debug.LogWarning("Starting client trhead, waiting for pong!");



        try
        {
            int recv = server.Receive(data);
            string stringData = Encoding.ASCII.GetString(data, 0, recv);

            //Debug.Log("Client: received data is:" + stringData);
            if (stringData == "pong")
            {
                Debug.Log("Server sent:    PONG");
                Thread.Sleep(500);
                clientState = stateTCP.send;
                count++;
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("Client: Connection failed.. trying again...");
            Debug.Log(e);
        }

        waitThreadCreated = false;
    }


    void ConnectThread()
    {
        try
        {
            server.Connect(ipep);
            Thread.Sleep(500);
        }
        catch (SocketException e)
        {
            Debug.Log("Client: Trying to connect to Server.");
            Debug.Log(e);
            return;
        }
        clientState = stateTCP.send;
    }

    void MoveMessages()
    {
        for(int i = 1; i>=messages.Length; i++)
        {
            messages[i] = messages[i - 1];
        }

        chat.transform.position += new Vector3(0, 13, 0);
    }

    void SpawnMessage(string name, string text, bool isSender)
    {
        MoveMessages();
        //message latestMessage = new message { name, text };
        //messages[0] = latestMessage;
        //if (!isSender)
        //{
        //    Instantiate(receivedmsg);
        //    receivedmsg.GetComponentInChildren<Text>
        //}
    }
}
