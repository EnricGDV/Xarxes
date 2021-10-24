﻿#define TCP_A

using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;




//################################################################################      UDP Client      ###########################################################################

#if UDP

public class CustomClient : MonoBehaviour
{

    enum state
    {
        none,
        send,
        await,
        shutDown
    }

    private byte[] data;
    private Thread waitThread;
    private IPEndPoint ipep;
    private Socket server;
    private state clientState;
    private int count = 0;
    private bool waitThreadCreated;
    // Start is called before the first frame update
    void Start()
    {
        clientState = state.send;

        data = new byte[1024];

        ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);

        server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        waitThreadCreated = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (count >= 5 && clientState != state.none)
            clientState = state.shutDown;

        switch (clientState)
        {
            case state.none:
                break;
            case state.send:
                SendPing();
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
                    //Debug.LogWarning("Client: thread is living la vida loca");
                }
                break;
            case state.shutDown:
                if (waitThread.IsAlive)
                    Debug.Log("Thread is Alive! Can't Shut Down!!!");
                Debug.Log("Stopping server from CustomClient.cs");
                clientState = state.none;
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
        //Debug.LogWarning("Starting client trhead, waiting for pong!");

        data = new byte[1024];

        try
        {
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)sender;
            int recv = server.ReceiveFrom(data, ref Remote);
            string stringData = Encoding.ASCII.GetString(data, 0, recv);
            //Debug.Log("Client: received data is:" + stringData);
            if (stringData == "pong")
            {
                Debug.Log("Server sent:    PONG");
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

        waitThreadCreated = false;
    }
}
#endif
#if TCP_A
    //################################################################################      TCP Client A      ###########################################################################
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

    private byte[] data;
    private Thread connectThread;
    private Thread waitThread;
    private IPEndPoint ipep;
    private Socket server;
    private stateTCP clientState;
    private int count;
    private bool waitThreadCreated;
    private bool connectThreadCreated;
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
            Debug.Log("Unable to connect to server.");
            Debug.Log(e);
            return;
        }
        clientState = stateTCP.send;
    }
}

#endif