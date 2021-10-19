using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class Client : MonoBehaviour
{
    private Thread myThread;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        myThread = new Thread(ThreadClient);
        myThread.Start();

    }

    void ThreadClient()
    {
        byte[] data = new byte[1024];
        string input, stringData;

        IPEndPoint ipep = new IPEndPoint(
                            IPAddress.Parse("127.0.0.1"), 9050);

        Socket server = new Socket(AddressFamily.InterNetwork,
                       SocketType.Dgram, ProtocolType.Udp);

        server.Bind(ipep);

        try
        {
            string welcome = "Hello, are you there?";
            data = Encoding.ASCII.GetBytes(welcome);
            server.SendTo(data, data.Length, SocketFlags.None, ipep);
        }
        catch (System.Exception e)
        {
            Debug.Log("Connection failed.. trying again...");
            Debug.Log(e);
        }


        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint Remote = (EndPoint)sender;


        try
        {
            data = new byte[1024];
            int recv = server.ReceiveFrom(data, ref Remote);
            Debug.Log("Message received from: " + Remote.ToString());
            Debug.Log(Encoding.ASCII.GetString(data, 0, recv));
        }
        catch (System.Exception e)
        {
            Debug.Log("Connection failed.. trying again...");
            Debug.Log(e);
        }


        while (true)
        {
            input = "ping";
            if (Input.GetKeyDown("space"))
            {
                break;
            }
                
            server.SendTo(Encoding.ASCII.GetBytes(input), Remote);
            data = new byte[1024];
            int recv = server.ReceiveFrom(data, ref Remote);
            stringData = Encoding.ASCII.GetString(data, 0, recv);
            Debug.Log(stringData);
        }
        Debug.Log("Stopping client");
        server.Close();
    }
}
