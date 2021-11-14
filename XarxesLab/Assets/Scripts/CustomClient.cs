using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.UIElements;
<<<<<<< Updated upstream
=======
using System.Collections.Generic;
using UnityEngine.SceneManagement;
>>>>>>> Stashed changes

public class CustomClient : MonoBehaviour
{
    enum stateTCP
    {
        none,
        connect,
        send_and_await,
        shutDown
    }

    struct message
    {
        public string name;
        public string text;

        public message(string nme, string txt) { name = nme; text = txt; }
    }

    private string clientWrittenMessage = "SEEEE";
    private string clientReceivedMessage;
    private bool MessageWritten;

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
    private GameObject[] panels;

    public GameObject chat;
    public GameObject receivedmsg;
    public GameObject sentmsg;
<<<<<<< Updated upstream
=======
    public GameObject inputfield;
    public int clientnum;
    private bool messagewritten;
    private string clientReceivedMessage;
>>>>>>> Stashed changes

    // Start is called before the first frame update
    void Start()
    {
        clientState = stateTCP.connect;

        data = new byte[1024];

        ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);

        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        waitThreadCreated = false;
        connectThreadCreated = false;
        MessageWritten = false;//we start with no messages written from client
        count = 0;

<<<<<<< Updated upstream
        messages = new message[1];
        panels = new GameObject[1];
=======
        clientnum = SceneManager.sceneCount - 1;
        messages = new List<message>();
        panels = new List<GameObject>();
        messagewritten = false;
        this.name = "client" + clientnum;
>>>>>>> Stashed changes
    }


    // Update is called once per frame
    void Update()
    {
        //if (count >= 5 && clientState != stateTCP.none)
        //    clientState = stateTCP.shutDown;

<<<<<<< Updated upstream
        //get input to write the message

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (clientWrittenMessage != null)
            {
                MessageWritten = true;
            }
            else
            {
                MessageWritten = false;
            }
=======
        if(Input.GetKeyDown(KeyCode.Return))
        {
            WriteMessage(inputfield.GetComponent<InputField>().text);
            messagewritten = true;
        }

        if(clientReceivedMessage != null)
        {
            SpawnMessage("OtherClient", clientReceivedMessage, false);
            clientReceivedMessage = null;
>>>>>>> Stashed changes
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
            case stateTCP.send_and_await:
                if (!waitThreadCreated)
                {
                    waitThread = new Thread(WaitThread);
                    waitThread.Start();
                    waitThreadCreated = true;
                }
<<<<<<< Updated upstream
                if (MessageWritten)
                {
                    //send message to the server
                    server.Send(Encoding.ASCII.GetBytes(clientWrittenMessage));
                    SpawnMessage("Client", clientWrittenMessage, true);
                    clientWrittenMessage = null;
                    MessageWritten = false;
=======
                if (messagewritten)
                {
                    //send message to the server
                    server.Send(Encoding.ASCII.GetBytes(messages[messages.Count-1].text));
                    messagewritten = false;
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream

=======
>>>>>>> Stashed changes
    void WaitThread()
    {
        try
        {
            int recv = server.Receive(data);
            string stringData = Encoding.ASCII.GetString(data, 0, recv);
<<<<<<< Updated upstream
            clientReceivedMessage = stringData;
            Debug.Log("message from server: " + clientReceivedMessage);
=======

            clientReceivedMessage = stringData;
>>>>>>> Stashed changes
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
        clientState = stateTCP.send_and_await;
<<<<<<< Updated upstream
        
=======
>>>>>>> Stashed changes
    }

    void MoveMessages()
    {
<<<<<<< Updated upstream
        //if (messages.Length < 10)
        //{
        //    messages = new message[messages.Length + 1];
        //}

        //if(messages.Length > 1)
        //{
        //    for (int i = 1; i < messages.Length; i++)
        //    {
        //        messages[i] = messages[i - 1];
        //        panels[i] = panels[i - 1];
        //        panels[i].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, panels[i].GetComponent<RectTransform>().rect.height);
        //    }
        //}
=======
        for (int i = 0; i < messages.Count; i++)
        {
            panels[i].GetComponent<RectTransform>().anchoredPosition
                = new Vector2(panels[i].GetComponent<RectTransform>().anchoredPosition.x, panels[i].GetComponent<RectTransform>().anchoredPosition.y + 15);
        }
    }

    public void SpawnMessage(string name, string text, bool isSender)
    {
        if (messages.Count > 0)
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
>>>>>>> Stashed changes
        
    }

    void SpawnMessage(string name, string text, bool isSender)
    {
<<<<<<< Updated upstream
        //MoveMessages();

        //if (!isSender)
        //{
        //    GameObject go = Instantiate(receivedmsg) as GameObject;
        //    go.GetComponent<MessageChildren>().childname.GetComponent<Text>().text = name;
        //    go.GetComponent<MessageChildren>().childtext.GetComponent<Text>().text = text;
        //    go.transform.SetParent(chat.transform, false);
        //    panels[0] = go;
        //}
        //else if (isSender)
        //{
        //    GameObject go = Instantiate(sentmsg) as GameObject;
        //    go.GetComponent<MessageChildren>().childname.GetComponent<Text>().text = name;
        //    go.GetComponent<MessageChildren>().childtext.GetComponent<Text>().text = text;
        //    go.transform.SetParent(chat.transform, false);
        //    panels[0] = go;
        //}



        //message latestMessage = new message(name, text);
        //messages[0] = latestMessage;
=======
        SpawnMessage(this.name, txt, true);
>>>>>>> Stashed changes
    }
}
