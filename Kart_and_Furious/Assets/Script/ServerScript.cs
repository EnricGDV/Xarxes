using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

public class ServerScript : MonoBehaviour // AKA: Server
{
    private Socket newSocket;
    private int recv;
    private IPEndPoint ipep;
    private Thread msgThread;
    private Thread pingThread; 
    private List<Player> playerList = new List<Player>();
    public int playerIt;
    public string localIPAddress;

    public enum ConnectionState
    {
        STATE_DISCONNECTED,
        STATE_HELLO,
        STATE_CONNECTED,
        STATE_DISCONNECTING
    }
    public class Player
    {
        public string playerName;
        public int id;
        private EndPoint endPoint;
        public ConnectionState connectionState = ConnectionState.STATE_DISCONNECTED;
        public DateTime lastPing;

        public Player(EndPoint ep, int id) { endPoint = ep; this.id = id; }
        public EndPoint GetEndPoint() { return endPoint; }
    }

    private void Awake()
    {
        playerIt = 1;

        localIPAddress = GetLocalIPv4();

        ipep = new IPEndPoint(IPAddress.Any, 2517);
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        newSocket.Bind(ipep);

        msgThread = new Thread(AwaitMsg);
        msgThread.Start();
    }

    void Start() 
    {
        pingThread = new Thread(Ping);
        pingThread.Start();
    }

    void Update()
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].connectionState == ConnectionState.STATE_CONNECTED)
            {
                DateTime now = System.DateTime.UtcNow;
                if ((now - playerList[i].lastPing) > TimeSpan.FromSeconds(5))
                {
                    playerList[i].connectionState = ConnectionState.STATE_DISCONNECTED;
                    Debug.Log(playerList[i].playerName + " disconnected!");
                }
            }
            else if (playerList[i].connectionState == ConnectionState.STATE_HELLO)
            {
                playerList[i].connectionState = ConnectionState.STATE_CONNECTED;
                SendMessageToClients("Welcome!" + playerList[i].id.ToString(), playerList[i]);
                playerList[i].lastPing = DateTime.UtcNow;
            }
        }
        if (Input.GetKeyDown(KeyCode.B)) // TODO: remove this
        {
            if (playerList.Count > 0)
                SendMessageToClients("B key pressed", playerList[0]);
        }
        else if (Input.GetKeyDown(KeyCode.N)) // TODO: remove this
        {
            if (playerList.Count > 0)
                SendMessageToClients("Disconnect!", playerList[0]);
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

                if (text.Contains("Ping!"))
                {
                    Player player = FindPlayerFromClient(ep);
                    player.lastPing = DateTime.UtcNow;
                    if (player.connectionState == ConnectionState.STATE_DISCONNECTED)
                    {
                        player.connectionState = ConnectionState.STATE_CONNECTED;
                        Debug.Log(player.playerName + " reconnected!");
                    }
                }
                else if (text == "Hello!") // This is the only one that can use ==
                {
                    AddPlayer(ep, playerIt);
                    Debug.Log("New client created: " + playerIt);
                    playerIt++;
                    Thread.Sleep(500);
                }
                else if (text.Contains("Goodbye!"))
                {
                    Player player = FindPlayerFromClient(ep);

                    if (player != null)
                    {
                        Debug.Log(player.playerName + " disconnected!");
                        playerList.Remove(player);
                    }
                }
                else if (text.Contains("Key"))
                {
                    Player player = FindPlayerFromClient(ep);
                    if (player != null)
                    {
                        SendMessageToClients(text, player);
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

    void Ping()
    {
        Debug.Log("Starting ping thread!");
        while (true) 
        {
            SendMessageToClients("Ping!");
            Thread.Sleep(500);
        }
    }

    // Sends a message to a single client or, if no client was specified, to all clients
    private void SendMessageToClients(string message, Player onlySendTo = null) 
    {
        if (playerList.Count < 1)
            return;

        byte[] data = Encoding.ASCII.GetBytes(message);

        if (onlySendTo == null)
        {
            for (int i = 0; i < playerList.Count; i++)
            {
                if (playerList[i].connectionState == ConnectionState.STATE_CONNECTED)
                    newSocket.SendTo(data, data.Length, SocketFlags.None, playerList[i].GetEndPoint());
            }
        }
        else if (onlySendTo.connectionState == ConnectionState.STATE_CONNECTED) 
            newSocket.SendTo(data, data.Length, SocketFlags.None, onlySendTo.GetEndPoint());
    }

    // Adds a player, lol
    private void AddPlayer(EndPoint ep, int id)
    {
        Player newPlayer = new Player(ep, id);
        playerList.Add(newPlayer);
        newPlayer.playerName = "New Player " + id.ToString();
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
    public string GetLocalIPv4()
    {
        return Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
    }

    // TODO: make sure this is needed
    private void OnDestroy()
    {
        if (msgThread != null)
            msgThread.Abort();
    }
}
