using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Collections.Generic;

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

        public message(string nme, string txt) { name = nme; text = txt; }
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
    private List<message> messages;
    private List<GameObject> panels;

    public GameObject chat;
    public GameObject content;
    public GameObject receivedmsg;
    public GameObject sentmsg;
    public GameObject inputfield;
    private bool messagewritten;

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

        messages = new List<message>();
        panels = new List<GameObject>();
        messagewritten = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (count >= 5 && clientState != stateTCP.none)
            clientState = stateTCP.shutDown;

        if(!messagewritten)
        {
            messagewritten = true;
            inputfield.GetComponent<InputField>().onEndEdit.AddListener(WriteMessage);
        }

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
                    SpawnMessage("Server", "PONG", false);
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
        SpawnMessage("Client", "PING", true);
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
        if(messages.Count > 0)
        {
            for (int i = 0; i < messages.Count; i++)
            {
                panels[i].GetComponent<RectTransform>().anchoredPosition
                    = new Vector2(panels[i].GetComponent<RectTransform>().anchoredPosition.x, panels[i].GetComponent<RectTransform>().anchoredPosition.y + 15);
            }
        }
    }

    public void SpawnMessage(string name, string text, bool isSender)
    {
        MoveMessages();
        
        if (!isSender)
        {
            GameObject go = Instantiate(receivedmsg, chat.transform) as GameObject;
            go.GetComponent<MessageChildren>().childname.GetComponent<Text>().text = name;
            go.GetComponent<MessageChildren>().childtext.GetComponent<Text>().text = text;
            go.GetComponent<RectTransform>().anchoredPosition
                    = new Vector2(go.GetComponent<RectTransform>().anchoredPosition.x, go.GetComponent<RectTransform>().anchoredPosition.y - 500);
            panels.Add(go);
        }
        else if (isSender)
        {
            GameObject go = Instantiate(sentmsg, chat.transform) as GameObject;
            go.GetComponent<MessageChildren>().childname.GetComponent<Text>().text = name;
            go.GetComponent<MessageChildren>().childtext.GetComponent<Text>().text = text;
            go.GetComponent<RectTransform>().anchoredPosition
                    = new Vector2(go.GetComponent<RectTransform>().anchoredPosition.x, go.GetComponent<RectTransform>().anchoredPosition.y - 500);
            panels.Add(go);
        }
        

        message latestMessage = new message (name, text);
        messages.Add(latestMessage);
        
    }

    void WriteMessage(string txt)
    {
        SpawnMessage("Client", txt, true);
    }
}
