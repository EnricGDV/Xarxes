using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

public class ConnectionAttempt2 : MonoBehaviour // AKA: Server
{
    private Socket newSocket;
    private int recv;
    private byte[] data;
    private IPEndPoint ipep;
    private string text;
    private Thread msgThread;
    private EndPoint client;
    private bool isNextMsgNeeded = false;
    private List<Player> playerList = new List<Player>();
    public class Player
    {
        public string playerName;
        public byte[] data; //TODO: maybe not needed
        public string textBuffer; //TODO: maybe not needed
        public bool isConnected = false; //TODO: maybe it should be set to true on constructor
        private EndPoint endPoint;
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
        if (Input.GetKeyDown(KeyCode.N))
        {
            SendMessage();
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

                if (text == "Hello")
                {
                    Player newPlayer = new Player(client);
                    playerList.Add(newPlayer);
                    newPlayer.playerName = "New Player 1";
                    newPlayer.isConnected = true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
                throw e;
            }
        }
    }

    void SendMessage()
    {
        if (playerList.Count < 1)
            return;

        Player player = playerList[0];
        data = Encoding.ASCII.GetBytes("Tomás");
        newSocket.SendTo(data, data.Length, SocketFlags.None, player.GetEndPoint());

        //msgThread = new Thread(AwaitMsg);
        //msgThread.Start();

        //isNextMsgNeeded = false;
    }
}
