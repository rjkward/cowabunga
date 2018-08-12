using System;
using Enums;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action<GameState> StateChanged;
    public static GameState CurrentState = GameState.Entry;
    [SerializeField]
    private GameObject _splashScreen;
    [SerializeField]
    private GameObject _selectScreen;
    [SerializeField]
    private FloorManager _floorManager;
    [SerializeField]
    private Camera _mainCamera;

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
    }

    private void OnDisable()
    {
        InputManager.NewInput -= HandleNewInput;
    }

    private void HandleNewInput(InputManager input)
    {
        switch (CurrentState)
        {
            case GameState.Entry:
                if (input.Space && _floorManager.FloorLoaded)
                {
                    ChangeGameState(GameState.PlayerSelect);
                    _splashScreen.SetActive(false);
                    _selectScreen.SetActive(true);
                }
                
                break;
            case GameState.PlayerSelect:
                if (input.Space)
                {
                    ChangeGameState(GameState.Started);
                    _selectScreen.SetActive(false);
                    _floorManager.StartErosion();
                }
                
                break;
            case GameState.Started:
                break;
            case GameState.Ended:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
