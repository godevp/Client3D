using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public class StateManager : MonoBehaviour
{
    private static StateManager instance;
    public string lastlySentLogin = "";
    public static StateManager Instance
    {
        get { return instance; }
    }
    public GameState state;
    private GameObject login_panel, account_panel;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (GameObject.Find("Login_Panel"))
        {
            login_panel = GameObject.Find("Login_Panel");
        }
        if (GameObject.Find("Account_Panel"))
        {
            account_panel = GameObject.Find("Account_Panel");
            account_panel.SetActive(false);
        }
    }

    public static event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        instance = this;
    }

    public void JoinWorld()
    {
        UpdateGameState(GameState.gameState);
    }

    public void LogOut()
    {
        UpdateGameState(GameState.logingState);
        TCP_Client.Instance.SendMessageToServer(MessageProcessing.Instance.Login + ':' + TCPClientToHost.DISCONNECT);
    }
public void UpdateGameState(GameState newState)
    {
        state = newState;
        

        switch(newState)
        {
            case GameState.logingState:
                SceneManager.LoadScene(0);
                break;
            case GameState.accountState:
                login_panel.SetActive(false);
                account_panel.SetActive(true);
                MessageProcessing.Instance.Login = lastlySentLogin;
                break;
            case GameState.gameState:
                SceneManager.LoadScene(1);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        OnGameStateChanged?.Invoke(newState);
    }

}


public enum GameState
{
    logingState,
    accountState,
    gameState
}