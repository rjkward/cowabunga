using System;
using Enums;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static event Action<GameState> StateChanged;
    public static GameState CurrentState = GameState.Entry;
    
    [SerializeField]
    private FloorManager _floorManager;

    private bool _enoughPlayers;

    private static void ChangeGameState(GameState newState)
    {
        CurrentState = newState;
        if (StateChanged != null)
        {
            StateChanged.Invoke(newState);
        }
    }

    private void OnEnable()
    {
        InputManager.NewInput += HandleNewInput;
        PlayerManager.EnoughPlayers += HandleEnoughPlayers;
        PlayerManager.OnePlayerRemaining += HandleOneRemaining;
    }

    private void OnDisable()
    {
        InputManager.NewInput -= HandleNewInput;
        PlayerManager.EnoughPlayers -= HandleEnoughPlayers;
        PlayerManager.OnePlayerRemaining -= HandleOneRemaining;
    }

    private void HandleOneRemaining(Rob_CharacterController remaining)
    {
        ChangeGameState(GameState.Ended);
    }

    private void HandleNewInput(InputManager input)
    {
        switch (CurrentState)
        {
            case GameState.Entry:
                if (input.Space && _floorManager.FloorLoaded)
                {
                    ChangeGameState(GameState.PlayerSelect);
                }
                
                break;
            case GameState.PlayerSelect:
                if (input.Space && _enoughPlayers)
                {
                    ChangeGameState(GameState.Started);
                    _floorManager.StartErosion();
                }
                
                break;
            case GameState.Started:
                break;
            case GameState.Ended:
                if (input.Space)
                {
                    ChangeGameState(GameState.Entry);
                    Scene s = SceneManager.GetActiveScene();
                    SceneManager.LoadScene(s.name);
                }
                
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void HandleEnoughPlayers()
    {
        _enoughPlayers = true;
    }
}
