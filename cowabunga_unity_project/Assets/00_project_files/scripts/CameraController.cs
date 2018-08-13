using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Camera _mainCamera;

    [SerializeField]
    private Transform _player1Spawn;
    
    [SerializeField]
    private Transform _defaultCameraPos;

    [SerializeField]
    private Transform _targetPosition;

    private Vector3 _currentLook;
    private Vector3 _lookTarget;

    private IList<Rob_CharacterController> _players;

    private Vector3 _mapCentre;

    private void OnEnable()
    {
        PlayerManager.PlayerCreated += HandlePlayerCreated;
        GameManager.StateChanged += HandleStateChanged;
        FloorManager.BroadcastPivot += HandleBroadcastPivot;

    }

    private void HandleBroadcastPivot(Vector3 pivot)
    {
        _mapCentre = pivot;
    }

    private void OnDisable()
    {
        PlayerManager.PlayerCreated -= HandlePlayerCreated;
        GameManager.StateChanged -= HandleStateChanged;
        FloorManager.BroadcastPivot -= HandleBroadcastPivot;
    }

    private void HandlePlayerCreated(IList<Rob_CharacterController> playerList)
    {
        _players = playerList;
        Vector3 target = Vector3.zero;
        for (int i = 0; i < playerList.Count; i++)
        {
            Rob_CharacterController player = playerList[i];
            target += player.transform.position;
        }

        target /= (float)playerList.Count;

        _lookTarget = target;
    }

    private void HandleStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.Entry:
                break;
            case GameState.PlayerSelect:
                _lookTarget = _mapCentre;
                _currentLook = _lookTarget;
                _targetPosition.position = _defaultCameraPos.position;
                _distance = _defaultCameraPos.position - _mapCentre;
                _rotatePivot = _mapCentre;
                StartCoroutine(SmoothDamp());
                StartCoroutine(Rotate());
                break;
            case GameState.Started:
                StartCoroutine(TrackPlayers());
                break;
            case GameState.Ended:
                for (int i = 0; i < _players.Count; i++)
                {
                    Rob_CharacterController player = _players[i];
                    if (player.transform.position.y > -2f)
                    {
                        _rotatePivot = player.transform.position;
                    }
                }
                
                _distance = new Vector3(0f, 1f, 3f);
                break;
            default:
                throw new ArgumentOutOfRangeException("newState", newState, null);
        }
    }

    private Vector3 _velocity;
    private Vector3 _lookVelocity;
    private Vector3 _distance;
    private Vector3 _rotatePivot;

    private IEnumerator TrackPlayers()
    {
        while (true)
        {
            int alive = 0;
            Vector3 lookTarget = Vector3.zero;
            for (int i = 0; i < _players.Count; i++)
            {
                Rob_CharacterController player = _players[i];
                if (player.transform.position.y > -2f)
                {
                    alive++;
                    lookTarget += player.transform.position;
                }
            }
    
            if (alive > 0)
            {
                lookTarget /= (float)alive;
                _lookTarget = lookTarget;
            }

            yield return null;
        }
    }

    private IEnumerator SmoothDamp()
    {
        while (true)
        {   
            _mainCamera.transform.position = Vector3.SmoothDamp(
                _mainCamera.transform.position,
                _targetPosition.position,
                ref _velocity,
                0.15f);
            
            _currentLook = Vector3.SmoothDamp(
                _currentLook,
                _lookTarget,
                ref _lookVelocity,
                0.3f);
            
            _mainCamera.transform.LookAt(_currentLook);
            
            
    
            yield return null;
        }
    }

    private IEnumerator Rotate()
    {
        while (true)
        {
            _targetPosition.position = _rotatePivot + Quaternion.Euler(0f, Time.time * 20f, 0f) * _distance;
            yield return null;
        }
    }
    
}
