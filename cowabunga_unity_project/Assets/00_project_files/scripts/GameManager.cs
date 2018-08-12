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
        _floorManager.FloorLoaded += OnLoadFloor;
    }

    private void OnDisable()
    {
        _floorManager.FloorLoaded -= OnLoadFloor;
    }

    private void Start()
    {
        _floorManager.GenerateFloor();
    }
    
    private void OnLoadFloor()
    {
        ChangeGameState(GameState.PlayerSelect);
        _splashScreen.SetActive(false);
        _floorManager.StartErosion();
    }
}
