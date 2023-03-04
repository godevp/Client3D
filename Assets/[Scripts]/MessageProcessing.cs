using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public struct HostToClient
{
    public const int CLIENT_CONNECTED = 1;
    public const int THE_NAME_IS_USED = 2;
    public const int ADD_PLAYER_TO_SCREEN = 3;
    public const int SEND_PLAYERDEST_TO_CLIENT = 4;
    public const int SEND_PLAYER_POS_DEST_TO_CLIENT = 5;

}

public struct ClientToHost
{
    public const int CONNECT = 1;
    public const int DISCONNECT = 2;
    public const int SEND_MY_DESTINATION = 3;
    public const int SEND_MY_POS_AND_DEST = 4;
    
}
public class MessageProcessing : MonoBehaviour
{
    private bool createNew = false;
    private Vector3 pos = new Vector3();
    private Vector3 dest11 = new Vector3();
    private string nameForNewPlayer = "";
    private Player _player;

    private static MessageProcessing instance;

    public static MessageProcessing Instance
    {
        get { return instance;  }
    }
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        instance = this;
        if(FindObjectOfType<Player>() != null)
            _player = FindObjectOfType<Player>();
    }

    void Update()
    {
        CreateNewPlayerOnTheirConnect();
    }

    void CreateNewPlayerOnTheirConnect()
    {
        if (createNew)
        {
            GameObject playerobject = (GameObject)Instantiate(Resources.Load("Prefabs/ConnectedPlayer"));
            Player newPlayer = playerobject.GetComponent<Player>();
            playerobject.transform.position = new Vector3(pos.x, pos.y, pos.z);
            newPlayer.destPos = new Vector3(dest11.x,playerobject.transform.position.y,dest11.z);
            newPlayer._name = nameForNewPlayer;
            UDP_Client.instance.otherPlayers.Add(newPlayer);
            Vector3 pos1 = _player.transform.position;
            Vector3 dest = _player.destPos;
            // SendUDPMessage(UDP_Client.instance._name + ':' + ClientToUdpHost.SEND_MY_POS_AND_DEST + ':' +
            //                pos1.x + ',' + pos1.y + ',' + pos1.z + ':' +
            //                dest.x + ',' + dest.y + ',' + dest.z);
            createNew = false;
            pos = new Vector3();
            dest11 = new Vector3();
        }
    }
    
    public void MSGProcessingBytoToString(string msg)
    {
        Debug.Log("Received message: " + msg);
        if (msg.Length > 0)
        {
            string[] splitter = msg.Split(':');
            int k = 0;
            int identifier = 0;
            if (int.TryParse(splitter[0], out k))
            {
                identifier = int.Parse(splitter[0]);
            }
            switch (identifier)
            {
                
                
                
                default: break;
            }
        }
    }

    private static void SendTCPMessage(string msg)
    {
        TCP_Client.Instance.SendMessageToServer(msg);
    }
}
