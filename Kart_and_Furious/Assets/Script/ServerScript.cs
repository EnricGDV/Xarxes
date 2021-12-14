using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

public class ServerScript : MonoBehaviour // AKA: Server
{
    private Socket newSocket;
    private int recv;
    private byte[] data;
    private IPEndPoint ipep;
    private string text;
    private Thread msgThread;
    private EndPoint client;
    private List<Player> playerList = new List<Player>();
    
    public enum ConnectionState
    {
        STATE_NONE,
        STATE_HELLO,
        STATE_CONNECTED,
        STATE_DISCONNECTING
    }
    public class Player
    {
        public string playerName; 
        public byte[] data; //TODO: maybe not needed
        public string textBuffer; //TODO: maybe not needed
        private EndPoint endPoint;
        public ConnectionState connectionState = ConnectionState.STATE_NONE;
        public Player(EndPoint ep) { endPoint = ep; }
        public EndPoint GetEndPoint() { return endPoint; }
    }

    // Start is called before the first frame update
    void Start() 
    {
        data = new byte[1024];
        ipep = new IPEndPoint(IPAddress.Any, 2517);
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        newSocket.Bind(ipep);

        msgThread = new Thread(AwaitMsg);
        msgThread.Start();
    }

    void Update()
    {
        for(int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].connectionState == ConnectionState.STATE_HELLO)
            {
                SendMessage(playerList[i], "Welcome!");
                playerList[i].connectionState = ConnectionState.STATE_CONNECTED;
            }
        }
        if (Input.GetKeyDown(KeyCode.B)) // TODO: remove this
        {
            if (playerList.Count > 0)
                SendMessage(playerList[0], "B pressed");
        }
        else if (Input.GetKeyDown(KeyCode.N)) // TODO: remove this
        {
            if (playerList.Count > 0)
                SendMessage(playerList[0], "Disconnect!");
        }
    }
    void AwaitMsg()
    {
        Debug.Log("Starting server thread! Awaiting messages...");
        while (true)
        {
            try
            {
                client = ipep;
                recv = newSocket.ReceiveFrom(data, ref client);
                text = Encoding.ASCII.GetString(data, 0, recv);
                Debug.Log(text);

                if (text == "Hello!")
                {
                    Player newPlayer = new Player(client);
                    playerList.Add(newPlayer);
                    newPlayer.playerName = "New Player 1";
                    newPlayer.connectionState = ConnectionState.STATE_HELLO;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
                throw e;
            }
        }
    }

    void SendMessage(Player client, string message)
    {
        if (playerList.Count < 1)
            return;

        data = Encoding.ASCII.GetBytes(message);
        newSocket.SendTo(data, data.Length, SocketFlags.None, client.GetEndPoint());

        //msgThread = new Thread(AwaitMsg);
        //msgThread.Start();

        //isNextMsgNeeded = false;
    }
}
