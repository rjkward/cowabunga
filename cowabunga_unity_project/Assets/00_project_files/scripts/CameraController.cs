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
        Debug.Log(_mapCentre.ToString());
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
        
        Debug.Log(target);

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
                StartCoroutine(SmoothDamp());
                break;
            case GameState.Started:
                break;
            case GameState.Ended:
                break;
            default:
                throw new ArgumentOutOfRangeException("newState", newState, null);
        }
    }

    private Vector3 _velocity;
    private Vector3 _lookVelocity;

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
                0.15f);
            
            _mainCamera.transform.LookAt(_currentLook);
            
            
    
            yield return null;
        }
    }
    
}
