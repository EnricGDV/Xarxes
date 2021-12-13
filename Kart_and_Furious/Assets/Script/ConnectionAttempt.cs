using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Threading;

public class ConnectionAttempt : MonoBehaviour
{
    private Socket newSocket;
    private byte[] data;
    private IPEndPoint ipep;
    private int recv;
    private string text;
    private Thread msgThread;
    static string iPAddress = "192.168.204.24";

    // Start is called before the first frame update
    void Start()
    {
        //string ipAd = GetLocalIPv4();
        ipep = new IPEndPoint(IPAddress.Parse(iPAddress), 2517);
        data = new byte[1024];
        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        data = Encoding.ASCII.GetBytes("Hello");
        newSocket.SendTo(data, data.Length, SocketFlags.None, ipep);
        msgThread = new Thread(AwaitMsgUDP);
        msgThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AwaitMsgUDP()
    {
        Debug.Log("Starting client thread! Awaiting message from server...");

        data = new byte[1024];
        try
        {
            EndPoint server = ipep;
            recv = newSocket.ReceiveFrom(data, ref server);
            text = Encoding.ASCII.GetString(data, 0, recv);
            Debug.Log(text);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e);
            throw e;
        }
        Debug.Log("Message received. Stopping client thread!");
    }
    public string GetLocalIPv4()
    {
        return Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
    }
}
