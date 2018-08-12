using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private Rob_CharacterController _playerPrefab;
    private readonly List<Rob_CharacterController> _playerList = new List<Rob_CharacterController>();
    private readonly Dictionary<KeyCode, Rob_CharacterController> _playerDict = new Dictionary<KeyCode, Rob_CharacterController>();
    [SerializeField]
    private Transform _player1Spawn;

    private float _spawnOffset = 2f;

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
        switch (GameManager.CurrentState)
        {
            case GameState.Entry:
                break;
            case GameState.PlayerSelect:
                CreateNewPlayers(input);
                break;
            case GameState.Started:
                break;
            case GameState.Ended:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

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
}
