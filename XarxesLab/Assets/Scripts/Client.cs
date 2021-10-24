
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
        myThread = new Thread(ThreadClient);
        if (!myThread.IsAlive)
        {
            myThread.Start();
        }
        else if (myThread.IsAlive)
        {
            myThread.Abort();
        }

    }

    // Update is called once per frame
    void Update()
    {


    }

    public void OnApplicationQuit()
    {
        if (Application.isEditor)
        {
            // signal your threads to exit
            if (myThread.IsAlive)
            {
                Debug.Log("client threads abort");
                myThread.Abort();
            }
        }
    }
    public void OnDestroy()
    {
        // signal your threads to exit
        if (myThread.IsAlive)
        {
            Debug.Log("client threads abort");
            myThread.Abort();           
        }
    }
    void ThreadClient()
    {
        int counter = 0;
        byte[] data = new byte[1024];
        string input, stringData;

        IPEndPoint ipep = new IPEndPoint(//class that represents a network endpoint as and IP and a port.
                            IPAddress.Parse("127.0.0.1"), //  The IP address of the Internet host. For example, the value 0x2414188f in big - endian format would be the IP address "143.24.20.36".
                            9050); //The port number associated with the address, or 0 to specify any available port. port is in host order.

        Socket server = new Socket(AddressFamily.InterNetwork,  //The AddressFamily enumeration specifies the standard address families used by the Socket class to resolve network addresses (for example, the AddressFamily.InterNetwork member specifies the IP version 4 address family).
                       SocketType.Dgram, ProtocolType.Udp);

        server.Bind(ipep);

        try
        {
            string welcome = "Client says: Hello, are you there?";
            data = Encoding.ASCII.GetBytes(welcome);
            server.SendTo(data, data.Length, SocketFlags.None, ipep);

        }
        catch (System.Exception e)// class exeption type ois copied in e and ready to display the exeption when desired
        {
            Debug.Log("Client: Connection failed.. trying again...");
            Debug.Log(e);
        }


        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint Remote = (EndPoint)sender;


        try
        {
            data = new byte[1024];
            int recievedData = server.ReceiveFrom(data, ref Remote);
            Debug.Log("Client: Message received from: " + Remote.ToString());
            Debug.Log("Client: data recieved" + Encoding.ASCII.GetString(data, 0, recievedData));

        }
        catch (System.Exception e)
        {
            Debug.Log("Client: Connection failed.. trying again...");
            Debug.Log(e);
        }


        while (true)
        {
            if (counter > 5)
            {
                break;
            }
            input = "ping";

            server.SendTo(Encoding.ASCII.GetBytes(input), Remote);
            data = new byte[1024];
            int recievedData = server.ReceiveFrom(data, ref Remote);
            stringData = Encoding.ASCII.GetString(data, 0, recievedData);
            Debug.Log(stringData);
            counter++;
        }
        Debug.Log("Stopping server from client.cs");
        server.Close();
    }
}
