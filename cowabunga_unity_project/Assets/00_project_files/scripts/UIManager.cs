using System;
using Enums;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _splashScreen;
    [SerializeField]
    private GameObject _selectScreen;
    [SerializeField]
    private GameObject _startPrompt;
    [SerializeField] 
    private GameObject _victoryScreen;

    private void OnEnable()
    {
        GameManager.StateChanged += HandleStateChanged;
        PlayerManager.EnoughPlayers += HandleEnoughPlayers;
    }

    private void OnDisable()
    {
        GameManager.StateChanged -= HandleStateChanged;
        PlayerManager.EnoughPlayers -= HandleEnoughPlayers;
    }

    private void HandleEnoughPlayers()
    {
        if (!_startPrompt.activeSelf)
        {
            _startPrompt.SetActive(true);
        }
    }

    private void HandleStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.Entry:
                break;
            case GameState.PlayerSelect:
                _splashScreen.SetActive(false);
                _selectScreen.SetActive(true);
                break;
            case GameState.Started:
                _selectScreen.SetActive(false);
                break;
            case GameState.Ended:
                _victoryScreen.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException("newState", newState, null);
        }
    }
}
