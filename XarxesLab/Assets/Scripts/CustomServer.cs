#define TCP_B

using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

class Client
{
    ~Client()
    {
        client_socket.Close();
    }
    public IPEndPoint clientep;
    public string name;
    public Socket client_socket;
}


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

    string messageFromClient;

    private byte[] data;
    private string input;
    private IPEndPoint ipep;
    private Socket newsocket;
    private Socket client;
    private Client newClient;
    private List<Client> clients_list;
    private Thread waitThread;
    private Thread listenThread;
    private stateTCP serverState;
    private IPEndPoint clientep;  //AKA sender;

    private bool newClientConnected = false;//when a new client connect, this turns true and a message of connection succeed is sent
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

        clients_list = new List<Client>();
    }

    // Update is called once per frame
    void Update()
    {
        if(messageFromClient != null)
        {

        }

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
                break;
            case stateTCP.send:
                if (newClientConnected)
                {
                    int elements = clients_list.Count;
                    clients_list[elements-1].client_socket.Send(Encoding.ASCII.GetBytes("connection succed, welcome to the Chat Room"));                   
                    newClientConnected = false;
                    serverState = stateTCP.await;
                }
                break;
            case stateTCP.shutDown:
                if (waitThread.IsAlive)
                    Debug.Log("Thread is Alive! Can't Shut Down!!!");

                for(int i = 0; i < clients_list.Count; i++)
                {
                    clients_list.RemoveAt(i);
                }

                //client.Close();
                newsocket.Close();
                serverState = stateTCP.none;

                break;
        }
    }


    void WaitThread() // wait for an input from any client
    {

        data = new byte[1024];

        try
        {
            int recv = client.Receive(data);
            string stringData = Encoding.ASCII.GetString(data, 0, recv);
            messageFromClient = stringData;
            
        }
        catch (System.Exception e)
        {
            Debug.Log("Server: Connection failed... trying again...");
            Debug.Log(e);
        }

        waitThreadCreated = false;
        
        serverState = stateTCP.send;
    }

    void ListenThread() //accept clients and connect them
    {
        try
        {
            Debug.Log("Server: Starting listening thread, Waiting for a client...");
            newsocket.Listen(10);
            Thread.Sleep(500);

            newClient = new Client();
            newClient.client_socket = newsocket.Accept();
            newClient.clientep = (IPEndPoint)newClient.client_socket.RemoteEndPoint;
            
            clients_list.Add(newClient);
            
            //TODO: SEND MESSAGE OF CONECTION SUCCESS TO THE CLIENT
        }
        catch (System.Exception e)
        {
            Debug.Log("Server: Trying to look for clients");
            Debug.Log(e);
        }
        serverState = stateTCP.await;
        newClientConnected = true;
    }

}




