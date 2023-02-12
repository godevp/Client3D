using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;



public struct UdpHostToClient
{
    public const int CLIENT_CONNECTED = 1;
    public const int THE_NAME_IS_USED = 2;
    public const int ADD_PLAYER_TO_SCREEN = 3;
    public const int SEND_PLAYERDEST_TO_CLIENT = 4;
    public const int SEND_PLAYER_POS_DEST_TO_CLIENT = 5;
}

public struct ClientToUdpHost
{
    public const int CONNECT = 1;
    public const int DISCONNECT = 2;
    public const int SEND_MY_DESTINATION = 3;
    public const int SEND_MY_POS_AND_DEST = 4;
}

public class ClientMessageProcessing : MonoBehaviour
{
    private bool createNew = false;
    private Vector3 pos = new Vector3();
    private Vector3 dest11 = new Vector3();
    private string nameForNewPlayer = "";
    private Player _player;

    private void Start()
    {
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
            SendUDPMessage(UDP_Client.instance._name + ':' + ClientToUdpHost.SEND_MY_POS_AND_DEST + ':' +
                           pos1.x + ',' + pos1.y + ',' + pos1.z + ':' +
                           dest.x + ',' + dest.y + ',' + dest.z);
            createNew = false;
            pos = new Vector3();
            dest11 = new Vector3();
        }
    }
    
    public void MSGProcessingBytoToString(byte[] msg)
    {
        string messageReceived = Encoding.UTF8.GetString(msg);
        if (messageReceived.Length > 0)
        {
            string[] splitter = messageReceived.Split(':');
            int k = 0;
            int identifier = 0;
            if (int.TryParse(splitter[0], out k))
            {
                identifier = int.Parse(splitter[0]);
            }
            switch (identifier)
            {
                case UdpHostToClient.CLIENT_CONNECTED:
                    UDP_Client.instance.connected = true;
                    break;
                
                case UdpHostToClient.ADD_PLAYER_TO_SCREEN :
                    string [] xyz = splitter[2].Split(",");
                    pos = new Vector3(float.Parse(xyz[0]),float.Parse(xyz[1]), float.Parse(xyz[2]));
                    nameForNewPlayer = splitter[1];
                    createNew = true;
                    break;
                
                case UdpHostToClient.SEND_PLAYERDEST_TO_CLIENT:
                    string[] newDestXYZ = splitter[2].Split(',');
                    Vector3 destXYZ = new Vector3(float.Parse(newDestXYZ[0]),float.Parse(newDestXYZ[1]), float.Parse(newDestXYZ[2])) ;
                    foreach (Player player in UDP_Client.instance.otherPlayers)
                    {
                        if(player._name == splitter[1])
                            player.destPos = new Vector3(destXYZ.x, destXYZ.y, destXYZ.z);
                    }
                    break;
                
                case UdpHostToClient.SEND_PLAYER_POS_DEST_TO_CLIENT:
                    string[] newPosXYZ = splitter[2].Split(',');
                    Vector3 posXYZ = new Vector3(float.Parse(newPosXYZ[0]),float.Parse(newPosXYZ[1]), float.Parse(newPosXYZ[2])) ;
                    string[] newDestXYZ1 = splitter[3].Split(',');
                    Vector3 destXYZ1 = new Vector3(float.Parse(newDestXYZ1[0]),float.Parse(newDestXYZ1[1]), float.Parse(newDestXYZ1[2]));
                    bool containsThePlayer = false;
                    foreach (Player player in UDP_Client.instance.otherPlayers)
                    {
                        if (player._name == splitter[1])
                        {
                            containsThePlayer = true;
                            player.transform.position = new Vector3(posXYZ.x, player.transform.position.y, posXYZ.z);
                            player.destPos = new Vector3(destXYZ1.x, destXYZ1.y, destXYZ1.z);
                        }
                    }

                    if (!containsThePlayer)
                    {
                        pos = posXYZ;
                        dest11 = destXYZ1;
                        nameForNewPlayer = splitter[1];
                        createNew = true;
                    }
                    break;

            }
        }
        Debug.Log(messageReceived);
    }

    private static void SendUDPMessage(string message)
    {
         UDP_Client.instance.SendMessageToUDPHost(message);
    }
}
