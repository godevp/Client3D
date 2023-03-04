using System;
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

    private string loginText, passwordText;

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
        if(GameObject.Find("Login_f") != null)
            loginText = GameObject.Find("Login_f").GetComponent<TMPro.TMP_InputField>().text;
        if(GameObject.Find("Password_f") != null)
            passwordText = GameObject.Find("Password_f").GetComponent<TMPro.TMP_InputField>().text;

        if (stream != null && stream.DataAvailable)
        {
            int bytesRead = stream.Read(receiveBuffer, 0, receiveBuffer.Length);
            string messageFromServer = Encoding.ASCII.GetString(receiveBuffer, 0, bytesRead);
            //message processing
            MessageProcessing.Instance.MSGProcessingBytoToString(messageFromServer);
        }
    }

    public void LogInAccount()
    {
        SendMessageToServer(loginText + ':' + passwordText);
        StateManager.Instance.lastlySentLogin = loginText;
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
        }
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