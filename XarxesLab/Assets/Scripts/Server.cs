using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class Server : MonoBehaviour
{
    private Thread myThread;

    // Start is called before the first frame update
    void Start()
    {
        myThread = new Thread(ThreadServer);
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
                myThread.Abort();
            }
        }
    }
    public void OnDestroy()
    {
        // signal your threads to exit
        if (myThread.IsAlive)
        {
            myThread.Abort();
        }
    }
    void ThreadServer()
    {
        int recievedData;
        byte[] data = new byte[1024];
        string input, stringData;

        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);

        Socket newsocket = new Socket(AddressFamily.InterNetwork,
                         SocketType.Dgram, ProtocolType.Udp);

        newsocket.Bind(ipep);

        Debug.Log("Server: Waiting for a client...");

        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint Remote = (EndPoint)(sender);


        try
        {
            recievedData = newsocket.ReceiveFrom(data, ref Remote);//des de remote recibimos data y la guardamos

            Debug.Log("Server: Message received from: " + Remote.ToString());//remote es el cliente / sender
            Debug.Log("Encoding.ASCII.GetString(data, 0, recievedData) = " + Encoding.ASCII.GetString(data, 0, recievedData));//vemos q tenemos dentro
            
        }
        catch (System.Exception e)
        {
            Debug.Log("Server: Server failed.. trying again...");
            Debug.Log(e);
        }


        try
        {
            string welcome = "Server: Welcome to my test server";
            data = Encoding.ASCII.GetBytes(welcome);
            newsocket.SendTo(data, data.Length, SocketFlags.None, Remote);
        }
        catch (System.Exception e)
        {
            Debug.Log("Server: error with sending welcome string");
            Debug.Log(e);
        }

        
    

        while (true)
        {
            input ="pong";
            data = new byte[1024];
            recievedData = newsocket.ReceiveFrom(data, ref Remote);

            Debug.Log("Server: recieved data:"+Encoding.ASCII.GetString(data, 0, recievedData));


            if (data.Length > 0)
            {
                data = Encoding.ASCII.GetBytes(input);
            }


            newsocket.SendTo(data, recievedData, SocketFlags.None, Remote);

         
        }
       
    }
}
