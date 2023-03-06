using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using TMPro;
using Object = System.Object;

#region Identifiers
public struct TCPHostToClient
{
    public const int LOGGED_SUCCESSFULLY = 1;
    public const int LOGIN_DENIED = 2;
    public const int REGISTRATION_FAILED = 3;
    public const int REGISTRATION_APPROVED = 4;
    public const int CANT_CREATE_CHARACTER = 5;
    public const int CHARACTER_CREATED = 6;
    public const int NEW_CHARACTER_JOINED_SERVER = 7;
    public const int YOUR_POSITION = 8;
    
}

public struct TCPClientToHost
{
    public const int DISCONNECT = 1;
    public const int REGISTRATION = 2;
    public const int LOGIN = 3;
    public const int SAVE_NEW_CHARACTER = 4;
    public const int DELETE_CHARACTER = 5;
    public const int CHARACTER_JOINING_WORLD = 6;
}


#endregion
public class MessageProcessing : MonoBehaviour
{
    #region variables
    
    [Header("Account Part")] [SerializeField]
    private string userName, c1 = "", c2 ="";


    public Vector3 playerSpawnPos = new Vector3();

    private GameObject c1Name,c2Name, c1NameF, c2NameF, c1SaveB, c2SaveB, c1DeleteB, c2DeleteB, c1JoinB, c2JoinB, accountMSG;
    private string nameForNewPlayer = "";
    public List<Player> otherPlayers = new List<Player>();
    public string Login
    {
        set => userName = value;
        get => userName;
    }
    //for other players
    private bool createNew = false;
    private string nameForOtherPlayer = "";
    private Queue<OtherPlayer> otherPlayerQueue = new Queue<OtherPlayer>();
    //
    
    private static char separator = ':';
    private Player _player;
    private string loginText, passwordText;
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
        _player = (FindObjectOfType<Player>() != null) ? FindObjectOfType<Player>() : _player;
        c1Name = (GameObject.Find("C1Name") != null) ? GameObject.Find("C1Name") : null;
        c1Name.SetActive(false);
        c2Name = (GameObject.Find("C2Name") != null) ? GameObject.Find("C2Name") : null;
        c2Name.SetActive(false);
        
        c1NameF = (GameObject.Find("C1Name_f") != null) ? GameObject.Find("C1Name_f") : null;
        c1NameF.SetActive(false);
        c2NameF = (GameObject.Find("C2Name_f") != null) ? GameObject.Find("C2Name_f") : null;
        c2NameF.SetActive(false);
        
        c1SaveB = (GameObject.Find("C1Save_b") != null) ? GameObject.Find("C1Save_b") : null;
        c1SaveB.SetActive(false);
        c2SaveB = (GameObject.Find("C2Save_b") != null) ? GameObject.Find("C2Save_b") : null;
        c2SaveB.SetActive(false);
        
        c1JoinB = (GameObject.Find("C1JoinWorld_b") != null) ? GameObject.Find("C1JoinWorld_b") : null;
        c1JoinB.SetActive(false);
        c2JoinB = (GameObject.Find("C2JoinWorld_b") != null) ? GameObject.Find("C2JoinWorld_b") : null;
        c2JoinB.SetActive(false);
        
        c1DeleteB = (GameObject.Find("C1Delete_b") != null) ? GameObject.Find("C1Delete_b") : null;
        c1DeleteB.SetActive(false);
        c2DeleteB = (GameObject.Find("C2Delete_b") != null) ? GameObject.Find("C2Delete_b") : null;
        c2DeleteB.SetActive(false);
        
        accountMSG = (GameObject.Find("AccountMSG") != null) ? GameObject.Find("AccountMSG") : null;
        accountMSG.SetActive(false);
    }

    void Update()
    {
        loginText = (GameObject.Find("Login_f") != null) ? GameObject.Find("Login_f").GetComponent<TMPro.TMP_InputField>().text : loginText;
        passwordText = (GameObject.Find("Password_f") != null) ? GameObject.Find("Password_f").GetComponent<TMPro.TMP_InputField>().text : passwordText;

        CreateNewPlayerOnTheirConnect();
    }
    #endregion


  
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
                {
                    c1Name.SetActive(true);
                    c2Name.SetActive(true);
                    c1 = splitter[1];
                    c2 = splitter[2];
                    c1Name.GetComponent<TMP_Text>().text += c1;
                    c2Name.GetComponent<TMP_Text>().text += c2;
                    
                    StateManager.Instance.UpdateGameState(GameState.accountState);
                    if (splitter[1] == "")
                    {
                        //show input field and button save for this slot.
                        c1NameF.SetActive(true);
                        c1SaveB.SetActive(true);
                    }
                    else
                    {
                        //show button Join world
                        c1JoinB.SetActive(true);
                        c1DeleteB.SetActive(true);
                    }

                    if (splitter[2] == "")
                    {
                        //show input field and button save for this slot.
                        c2NameF.SetActive(true);
                        c2SaveB.SetActive(true);
                    }
                    else
                    {
                        //show button Join world
                        c2JoinB.SetActive(true);
                        c2DeleteB.SetActive(true);
                    }
                    break;
                }
                case TCPHostToClient.LOGIN_DENIED:
                {
                    if (GameObject.Find("ErrorMessage") != null && splitter.Length > 1)
                    {
                        GameObject.Find("ErrorMessage").SetActive(true);
                        GameObject.Find("ErrorMessage").GetComponent<TMPro.TMP_Text>().text = splitter[1];
                        GameObject.Find("ErrorMessage").GetComponent<TMPro.TMP_Text>().color = Color.red;
                    }
                    break;
                }
                case TCPHostToClient.REGISTRATION_FAILED:
                {
                    if (GameObject.Find("ErrorMessage") != null && splitter.Length > 1)
                    {
                        GameObject.Find("ErrorMessage").SetActive(true);
                        GameObject.Find("ErrorMessage").GetComponent<TMPro.TMP_Text>().text = splitter[1];
                        GameObject.Find("ErrorMessage").GetComponent<TMPro.TMP_Text>().color = Color.red;
                    }
                    break;
                }
                case TCPHostToClient.REGISTRATION_APPROVED:
                {
                    if (GameObject.Find("ErrorMessage") != null && splitter.Length > 1)
                    {
                        GameObject.Find("ErrorMessage").SetActive(true);
                        GameObject.Find("ErrorMessage").GetComponent<TMPro.TMP_Text>().text = "Registration completed";
                        GameObject.Find("ErrorMessage").GetComponent<TMPro.TMP_Text>().color = Color.green;

                        loginText = splitter[1];
                        passwordText = splitter[2];
                    }
                    break;
                }
                case TCPHostToClient.CANT_CREATE_CHARACTER:
                {
                    string err = splitter[1];
                    accountMSG.SetActive(true);
                    accountMSG.GetComponent<TMP_Text>().text = err;
                    accountMSG.GetComponent<TMP_Text>().color = Color.red;
                    break;
                }
                case TCPHostToClient.CHARACTER_CREATED:
                {
                    int slot = (int.TryParse(splitter[1], out _)) ? int.Parse(splitter[1]) : 0;
                    string nameForNewChar = splitter[2];
                    string err = splitter[3];
                    accountMSG.SetActive(true);
                    accountMSG.GetComponent<TMP_Text>().text = err;
                    accountMSG.GetComponent<TMP_Text>().color = Color.green;

                    switch (slot)
                    {
                        case 1:
                        {
                            c1 = nameForNewChar;
                            c1Name.SetActive(true);
                            c1JoinB.SetActive(true);
                            c1DeleteB.SetActive(true);
                            c1NameF.GetComponent<TMP_InputField>().text = "";
                            c1NameF.SetActive(false);
                            c1SaveB.SetActive(false);
                            c1Name.GetComponent<TMP_Text>().text += c1;
                            break;
                        }
                        case 2:
                        {
                            c2 = nameForNewChar;
                            c2Name.SetActive(true);
                            c2JoinB.SetActive(true);
                            c2DeleteB.SetActive(true);
                            c2NameF.GetComponent<TMP_InputField>().text = "";
                            c2NameF.SetActive(false);
                            c2SaveB.SetActive(false);
                            c2Name.GetComponent<TMP_Text>().text += c2;
                            break;
                        }
                        default: break;
                    }
                    
                    break;
                }
                case TCPHostToClient.YOUR_POSITION:
                {
                    StateManager.Instance.UpdateGameState(GameState.gameState);
                    string[] posSplitter = splitter[1].Split(',');
                    playerSpawnPos = new Vector3(float.Parse(posSplitter[0]), float.Parse(posSplitter[1]),
                        float.Parse(posSplitter[2]));
  
                    if (splitter.Length > 2)
                    {
                        for (int i = 2; i <= splitter.Length - 1; i++)
                        {
                            if (int.TryParse(splitter[i], out _))
                            {
                                int ident = int.Parse(splitter[i]);
                                if (ident != TCPHostToClient.NEW_CHARACTER_JOINED_SERVER)
                                    return;
                                
                                if (ident == TCPHostToClient.NEW_CHARACTER_JOINED_SERVER)
                                {
                                    string newPlayerName = splitter[i + 1];
                                    string[] vecSplitter = splitter[i + 2].Split(',');
                                    Vector3 thePlayerPos = new Vector3(float.Parse(vecSplitter[0]),
                                        float.Parse(vecSplitter[1]), float.Parse(vecSplitter[2]));
                                    string[] destVecSplitter = splitter[i + 3].Split(',');
                                    Vector3 thePlayerDestPos = new Vector3(float.Parse(destVecSplitter[0]),
                                        float.Parse(destVecSplitter[1]), float.Parse(destVecSplitter[2]));
                                    //create the player instance with the info provided.
                                    otherPlayerQueue.Enqueue(new OtherPlayer(newPlayerName, thePlayerPos, thePlayerDestPos));
                                }
                            }
                        }
                    }
                    break;
                }
                case TCPHostToClient.NEW_CHARACTER_JOINED_SERVER:
                {
                    string newPlayerName = splitter[1];
                    string[] vecSplitter = splitter[2].Split(',');
                    Vector3 thePlayerPos = new Vector3(float.Parse(vecSplitter[0]),
                        float.Parse(vecSplitter[1]), float.Parse(vecSplitter[2]));
                    string[] destVecSplitter = splitter[3].Split(',');
                    Vector3 thePlayerDestPos = new Vector3(float.Parse(destVecSplitter[0]),
                        float.Parse(destVecSplitter[1]), float.Parse(destVecSplitter[2]));
                    //StartNewPlayerCreation(thePlayerPos,thePlayerDestPos,newPlayerName);
                    
                    otherPlayerQueue.Enqueue(new OtherPlayer(newPlayerName, thePlayerPos, thePlayerDestPos));
                    break;
                }
                default: break;
            }
        }
    }
    
    /// <summary>
    /// This function will add new player to the game scene.
    /// </summary>
    void CreateNewPlayerOnTheirConnect()
    {
        if (otherPlayerQueue.Count > 0)
        {
            GameObject playerobject = (GameObject)Instantiate(Resources.Load("Prefabs/ConnectedPlayer"));
            Player newPlayer = playerobject.GetComponent<Player>();
            var tempPlayer = otherPlayerQueue.Dequeue();
            playerobject.transform.position = tempPlayer.position;
            newPlayer.destPos = tempPlayer.destPosition;
            newPlayer._name = tempPlayer.name;
            otherPlayers.Add(newPlayer);
            Debug.Log("Should Add: " + newPlayer._name);
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
    /// <summary>
    /// Message for login into account.
    /// </summary>
    public void LogInAccount()
    {
        SendTCPMessage(TCPClientToHost.LOGIN.ToString() + ':' + loginText + ':' + passwordText);
        StateManager.Instance.lastlySentLogin = loginText;
    }
    /// <summary>
    /// Registration logic.
    /// </summary>
    public void Registration()
    {
        SendTCPMessage(TCPClientToHost.REGISTRATION.ToString() + ':' + loginText + ':' + passwordText);
    }
    /// <summary>
    /// Sends a request to join the world.
    /// </summary>
    /// <param name="slot"></param>
    public void JoinWorld(int slot)
    {
        string characterName = "";
        switch (slot)
        {
            case 1:
            {
                characterName = c1;
                break;
            }
            case 2:
            {
                characterName = c2;
                break;
            }
            
         default: break;   
        }
        
        string[] elements = {TCPClientToHost.CHARACTER_JOINING_WORLD.ToString(),userName,slot.ToString(),characterName};
        SendTCPMessage(CombineWithSeparator(elements,separator.ToString()));
    }
    /// <summary>
    /// Deletes player from a specified character slot.
    /// </summary>
    /// <param name="slot"></param>
    public void DeleteCharacter(int slot)
    {
        string[] elements = {TCPClientToHost.DELETE_CHARACTER.ToString(),userName, slot.ToString()};
        SendTCPMessage(CombineWithSeparator(elements,separator.ToString()));
        switch (slot)
        {
            case 1:
            {
                c1Name.GetComponent<TMP_Text>().text = "Char1: ";
                c1Name.SetActive(false);
                c1DeleteB.SetActive(false);
                c1JoinB.SetActive(false);
                c1NameF.SetActive(true);
                c1SaveB.SetActive(true);
                break;
            }
            case 2:
            {
                c2Name.GetComponent<TMP_Text>().text = "Char2: ";
                c2Name.SetActive(false);
                c2DeleteB.SetActive(false);
                c2JoinB.SetActive(false);
                c2NameF.SetActive(true);
                c2SaveB.SetActive(true);
                break;
            }
            default: break;
        }
    }
    /// <summary>
    /// Used when we log out from account. To clear all the info on account panel.
    /// </summary>
    public void ClearAccount()
    {
        c1Name.GetComponent<TMP_Text>().text = "Char1: ";
        c2Name.GetComponent<TMP_Text>().text = "Char2: ";
    }

    public void SaveNewChar(int slot)
    {
        switch (slot)
        {
            case 1:
            {
                if(c1NameF.GetComponent<TMP_InputField>().text != "")
                    SendTCPMessage(TCPClientToHost.SAVE_NEW_CHARACTER.ToString() + ':' + userName + ':' +
                                   slot.ToString() + ':' + c1NameF.GetComponent<TMP_InputField>().text);
                break;
            }
            case 2:
            {
                if(c2NameF.GetComponent<TMP_InputField>().text != "")
                    SendTCPMessage(TCPClientToHost.SAVE_NEW_CHARACTER.ToString() + ':' + userName + ':' +
                                   slot.ToString() + ':' + c2NameF.GetComponent<TMP_InputField>().text);
                break;
            }
            default: break;
        }
      
    }
    /// <summary>
    /// Combines all the elements of string array and separate them with a certain string.
    /// </summary>
    /// <param name="variables"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    private static string CombineWithSeparator(string[] variables, string separator)
    {
        return string.Join(separator, variables);
    }
    
}
/// <summary>
/// A class which is used for easier adding of other players.
/// </summary>
public class OtherPlayer
{
    public string name;
    public Vector3 position;
    public Vector3 destPosition;
    public  OtherPlayer(string name, Vector3 position, Vector3 destPosition)
    {
        this.name = name;
        this.position = position;
        this.destPosition = destPosition;
    }
}