using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static event Action<Rob_CharacterController> OnePlayerRemaining;
    public static event Action EnoughPlayers;
    public static event Action<IList<Rob_CharacterController>> PlayerCreated;
    [SerializeField]
    private Rob_CharacterController _playerPrefab;
    private readonly List<Rob_CharacterController> _playerList = new List<Rob_CharacterController>();
    private readonly Dictionary<KeyCode, Rob_CharacterController> _playerDict = new Dictionary<KeyCode, Rob_CharacterController>();
    [SerializeField]
    private Transform _player1Spawn;

    private float _spawnOffset = 3f;

    private void OnEnable()
    {
        InputManager.NewInput += HandleNewInput;
        GameManager.StateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        InputManager.NewInput -= HandleNewInput;
        GameManager.StateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.Entry:
                break;
            case GameState.PlayerSelect:
                break;
            case GameState.Started:
                for (int i = 0; i < _playerList.Count; i++)
                {
                    _playerList[i].enabled = true;
                }

                StartCoroutine(CheckForWin());
                break;
            case GameState.Ended:
                StopAllCoroutines();
                HandleEndGame();
                break;
            default:
                throw new ArgumentOutOfRangeException("newState", newState, null);
        }
    }

    private void HandleEndGame()
    {
        for (int i = 0; i < _playerList.Count; i++)
        {
            _playerList[i].enabled = false;
        }
    }

    private void HandleNewInput(InputManager input)
    {
        switch (GameManager.CurrentState)
        {
            case GameState.Entry:
                break;
            case GameState.PlayerSelect:
                CreateNewPlayers(input);
                break;
            case GameState.Started:
                UpdateInput(input);
                break;
            case GameState.Ended:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void UpdateInput(InputManager input)
    {
        List<KeyCode> down = input.KeyDownList;
        int downCount = down.Count;
        for (int i = 0; i < downCount; i++)
        {
            Rob_CharacterController player;
            if (_playerDict.TryGetValue(down[i], out player))
            {
                player.SetInput(1f);
            }
        }
        
        List<KeyCode> up = input.KeyUpList;
        int upCount = up.Count;
        for (int i = 0; i < upCount; i++)
        {
            Rob_CharacterController player;
            if (_playerDict.TryGetValue(up[i], out player))
            {
                player.SetInput(0f);
            }
        }
    }

    private bool _enoughInvoked;

    private void CreateNewPlayers(InputManager input)
    {
        List<KeyCode> keysUp = input.KeyUpList;
        for (int i = 0; i < keysUp.Count; i++)
        {
            KeyCode key = keysUp[i];
            Rob_CharacterController player;
            if (_playerDict.TryGetValue(key, out player))
            {
                RerollPlayer(key);
            }
            else
            {
                CreateNewPlayer(key);
                if (PlayerCreated != null)
                {
                    PlayerCreated.Invoke(_playerList.AsReadOnly());
                }
                
                if (!_enoughInvoked && _playerList.Count > 1 && EnoughPlayers != null)
                {
                    EnoughPlayers.Invoke();
                    _enoughInvoked = true;
                }
            }
        }
    }

    private void CreateNewPlayer(KeyCode key)
    {
        Rob_CharacterController newPlayer = Instantiate(_playerPrefab, transform);
        newPlayer.transform.SetPositionAndRotation(
            _player1Spawn.position + (-_spawnOffset * _player1Spawn.right * _playerList.Count),
            _player1Spawn.rotation
        );
        _playerDict[key] = newPlayer;
        _playerList.Add(newPlayer);
    }
    
    private void RerollPlayer(KeyCode key)
    {
        // TODO
    }
    
    private readonly WaitForSeconds _wait = new WaitForSeconds(0.5f);

    private IEnumerator CheckForWin()
    {
        bool moreThanOnePlayerAlive = true;
        Rob_CharacterController last = _playerList[0];
        while (moreThanOnePlayerAlive)
        {
            int aliveCount = 0;
            for (int i = 0; i < _playerList.Count; i++)
            {
                Rob_CharacterController player = _playerList[i];
                if (!player.gameObject.activeSelf)
                {
                    continue;
                }

                if (player.transform.position.y < -50f)
                {
                    player.gameObject.SetActive(false);
                    continue;
                }

                if (player.transform.position.y < -2f)
                {
                    continue;
                }

                last = player;
                aliveCount++;
            }

            moreThanOnePlayerAlive = aliveCount > 1;
            yield return _wait;
        }

        if (OnePlayerRemaining != null)
        {
            OnePlayerRemaining.Invoke(last);
        }
    }
}
