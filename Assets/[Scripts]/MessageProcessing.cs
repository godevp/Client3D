using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Object = System.Object;

#region Identifiers
public struct TCPHostToClient
{
    public const int LOGGED_SUCCESSFULLY = 1;
    public const int LOGIN_DENIED = 2;

}

public struct TCPClientToHost
{
    public const int DISCONNECT = 1;

}
#endregion
public class MessageProcessing : MonoBehaviour
{
    #region variables
    
    [Header("Account Part")] [SerializeField] 
    private string userName;
    public string Login
    {
        set => userName = value;
        get => userName;
    }

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
    #endregion

    #region Awake, Start, Update
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
    #endregion
    /// <summary>
    /// This function will add new player to the game scene.
    /// </summary>
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
    /// <summary>
    /// Message Processing itself. Here will be decided what logic will be used after a certain message received from server.
    /// </summary>
    /// <param name="msg"></param>
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
                case TCPHostToClient.LOGGED_SUCCESSFULLY:
                    StateManager.Instance.UpdateGameState(GameState.accountState);
                    break;
                case TCPHostToClient.LOGIN_DENIED:
                    if (GameObject.Find("ErrorMessage") != null && splitter.Length > 1)
                    {
                        GameObject.Find("ErrorMessage").SetActive(true);
                        GameObject.Find("ErrorMessage").GetComponent<TMPro.TMP_Text>().text = splitter[1];
                    }
                    break;
                
                
                default: break;
            }
        }
    }
    
    
    /// <summary>
    /// Takes the function for sending TCP messages from TCP_Client. Just for easiest use.
    /// </summary>
    /// <param name="msg"></param>
    private static void SendTCPMessage(string msg)
    {
        TCP_Client.Instance.SendMessageToServer(msg);
    }
}
