using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    private static StateManager instance;

    public static StateManager Instance
    {
        get { return instance; }
    }
    public GameState state;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public static event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        instance = this;
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
                
                break;
            case GameState.gameState:
         
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