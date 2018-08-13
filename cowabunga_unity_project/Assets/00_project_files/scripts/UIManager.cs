using System;
using System.Collections;
using Enums;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] 
    private GameObject _startScreen;
    [SerializeField] 
    private Text _startText;

    private readonly string[] _startWords = new[]
        { "FIGHT", "TO", "THE", "DEATH", "YOU", "CAN", "ONLY", "TURN", "RIGHT"};

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
                StartCoroutine(SlideShowWords());
                break;
            case GameState.Ended:
                _victoryScreen.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException("newState", newState, null);
        }
    }

    readonly WaitForSeconds _wait = new WaitForSeconds(0.25f);

    private IEnumerator SlideShowWords()
    {
        _startScreen.SetActive(true);
        for (int i = 0; i < _startWords.Length; i++)
        {
            _startText.text = _startWords[i];
            yield return _wait;
        }
        
        _startScreen.SetActive(false);
    }
}
