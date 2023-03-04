using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using UnityEditor.PackageManager;
using System.Security.Cryptography;

public class UDP_Client : MonoBehaviour
{
    #region  variables
    //private:
    UdpClient udpClient;
    IPEndPoint RemoteIpEndPoint;
    Byte[] receiveBytes;
    Thread recevingThread;
    ClientMessageProcessing msgProcessing;
    //public:
    public string hostIP = "192.168.0.189";
    public int hostPort = 20001;
    public string _name = "";
    public bool connected = false;
    public static UDP_Client instance;
    public Player _player;
    public GameObject playerPrefab;
    public List<Player> otherPlayers = new List<Player>();
    #endregion
    
    void Start()
    {
        instance = this;
        _player = FindObjectOfType<Player>();
        udpClient = new UdpClient();
        this.gameObject.AddComponent<ClientMessageProcessing>();
        msgProcessing = this.gameObject.GetComponent<ClientMessageProcessing>();
        RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse(hostIP), hostPort);//To allow receiving messages from anywhere use (IPAdress.Any, 0)
        Connection();
        recevingThread = new Thread(new ThreadStart(BeginReceiving));
        recevingThread.IsBackground = true;
        recevingThread.Start();
    }
    #region receiving loop
    private void BeginReceiving()
    {
        try
        {
            udpClient.BeginReceive(new AsyncCallback(Receiving), null);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
    private void Receiving(IAsyncResult result)
    {
        byte[] received = udpClient.EndReceive(result, ref RemoteIpEndPoint);
        msgProcessing.MSGProcessingBytoToString(received);
        udpClient.BeginReceive(new AsyncCallback(Receiving), null);
    }
    #endregion

    private void Connection()
    {
            try
            {
                Vector3 pos = _player.transform.position;
                SendMessageToUDPHost(_name + ':' + ClientToUdpHost.CONNECT + ':'
                                     + pos.x + "," + pos.y + ',' + pos.z);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                StartCoroutine(TryReconnect());
            }
    }

    private IEnumerator TryReconnect()
    {
        yield return new WaitForSeconds(1.0f);
        Connection();
    }

    public void SendMessageToUDPHost(string message)
    {
        Byte[] sendBytes = Encoding.ASCII.GetBytes(message);
        udpClient.Send(sendBytes, sendBytes.Length,RemoteIpEndPoint);
    }

    private void OnApplicationQuit()
    {
        SendMessageToUDPHost(_name + ':' + ClientToUdpHost.DISCONNECT);
        udpClient.Close();
        recevingThread.Abort();
    }

}
