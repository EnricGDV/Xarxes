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
    private IPEndPoint ipep;
    private Thread msgThread;
    private List<Player> playerList = new List<Player>();
    public int playerIt = 1;

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
        public EndPoint endPoint;
        public ConnectionState connectionState = ConnectionState.STATE_NONE;
        public Player(EndPoint ep) { endPoint = ep; data = new byte[1024]; }
        public EndPoint GetEndPoint() { return endPoint; }
    }

    void Start() 
    {
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
                SendMessage("Welcome!", playerList[i]);
                playerList[i].connectionState = ConnectionState.STATE_CONNECTED;
            }
        }
        if (Input.GetKeyDown(KeyCode.B)) // TODO: remove this
        {
            if (playerList.Count > 0)
                SendMessage("B key pressed", playerList[0]);
        }
        else if (Input.GetKeyDown(KeyCode.N)) // TODO: remove this
        {
            if (playerList.Count > 0)
                SendMessage("Disconnect!", playerList[0]);
        }
    }

    void AwaitMsg()
    {
        Debug.Log("Starting server thread! Awaiting messages...");
        while (true)
        {
            try
            {
                byte[] data = new byte[1024];
                EndPoint ep = ipep;
                recv = newSocket.ReceiveFrom(data, ref ep);
                string text = Encoding.ASCII.GetString(data, 0, recv);

                if (text == "Hello!")
                {
                    AddPlayer(ep);
                }
                else if (text == "Goodbye!")
                {
                    Player newPlayer = FindPlayerFromClient(ep);

                    if (newPlayer != null)
                    {
                        Debug.Log(newPlayer.playerName + " disconnected!");
                        playerList.Remove(newPlayer);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
                throw e;
            }
        }
    }

    // Sends a message to a single client or, if no client was specified, to all clients
    private void SendMessage(string message, Player client = null) 
    {
        if (playerList.Count < 1)
            return;

        byte[] data = Encoding.ASCII.GetBytes(message);

        if (client == null)
        {
            for (int i = 0; i < playerList.Count; i++)
                newSocket.SendTo(data, data.Length, SocketFlags.None, playerList[i].GetEndPoint());
        }
        else
            newSocket.SendTo(data, data.Length, SocketFlags.None, client.GetEndPoint());
    }

    // Adds a player, lol
    private void AddPlayer(EndPoint ep)
    {
        Player newPlayer = new Player(ep);
        playerList.Add(newPlayer);
        newPlayer.playerName = "New Player " + playerIt.ToString(); playerIt++;
        newPlayer.connectionState = ConnectionState.STATE_HELLO;
    }

    // Returns the Player linked with the specified EndPoint
    private Player FindPlayerFromClient(EndPoint client) 
    {
        for(int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].GetEndPoint().ToString() == client.ToString())
                return playerList[i];
        }
        return null;
    }
}
