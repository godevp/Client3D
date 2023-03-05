using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class TCP_Client : MonoBehaviour
{
    public string serverIP = "192.168.0.189";
    public int serverPort = 20001;

    private TcpClient client;
    private NetworkStream stream;
    private byte[] receiveBuffer = new byte[1024];

   

    private static TCP_Client instance;

    public static TCP_Client Instance
    {
        get { return instance; }
    }
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        instance = this;
        ConnectToServer();
    }

    private void Update()
    {
        if (stream != null && stream.DataAvailable)
        {
            int bytesRead = stream.Read(receiveBuffer, 0, receiveBuffer.Length);
            string messageFromServer = Encoding.ASCII.GetString(receiveBuffer, 0, bytesRead);
            //message processing
            MessageProcessing.Instance.MSGProcessingBytoToString(messageFromServer);
        }
    }


    
    
    private void ConnectToServer()
    {
        try
        {
            client = new TcpClient(serverIP, serverPort);
            stream = client.GetStream();
        }
        catch (Exception e)
        {
            Debug.LogError("Error connecting to server: " + e.Message);
            StartCoroutine(TryReconnect());
        }
    }
    private IEnumerator TryReconnect()
    {
        yield return new WaitForSeconds(1.0f);
        ConnectToServer();
    }

    public void SendMessageToServer(string message)
    {
        try
        {
            byte[] messageBytes = Encoding.ASCII.GetBytes(message);
            stream.Write(messageBytes, 0, messageBytes.Length);
        }
        catch (Exception e)
        {
            Debug.LogError("Error sending message to server: " + e.Message);
        }
    }

    private void OnDestroy()
    {
        if (stream != null)
        {
            stream.Close();
        }

        if (client != null)
        {
            client.Close();
        }
    }
}