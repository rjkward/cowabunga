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
    
    public static event Action<KeyCode> StartIntro;
    public static event Action EndIntros;

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

    private void HandlePlayerCreated(KeyCode key, Rob_CharacterController newPlayer, IList<Rob_CharacterController> playerList)
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
        
        _queue.Add(new KeyPlayerPair(key, newPlayer));
        if (!_introsActive)
        {
            _introsActive = true;
            StartCoroutine(PlayIntros());
        }
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

    private bool _introsActive;
    private List<KeyPlayerPair> _queue = new List<KeyPlayerPair>();

    private class KeyPlayerPair
    {
        public KeyCode Key;
        public Rob_CharacterController Player;
        public KeyPlayerPair(KeyCode key, Rob_CharacterController player)
        {
            Key = key;
            Player = player;
        }
    }

    private IEnumerator PlayIntros()
    {
        while (_queue.Count > 0)
        {
            var info = _queue[0];
            _introLookTarget = info.Player.transform.position;
            if (StartIntro != null)
            {
                StartIntro.Invoke(info.Key);
            }
            
            _queue.RemoveAt(0);
            yield return _wait;
        }

        if (EndIntros != null)
        {
            EndIntros.Invoke();
        }
        
        _introsActive = false;
    }

    // private Vector3 _introRotatePivot;
    private Vector3 _introLookTarget;
    private Vector3 _introDistance = new Vector3(0f, 1f, 3f);
    private WaitForSeconds _wait = new WaitForSeconds(1f);
    

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
                _introsActive ? _introLookTarget : _lookTarget,
                ref _lookVelocity,
                _introsActive ? 0.1f : 0.3f);
            
            _mainCamera.transform.LookAt(_currentLook);
            
            
    
            yield return null;
        }
    }

    private IEnumerator Rotate()
    {
        while (true)
        {
            Vector3 pivot = _introsActive ? _introLookTarget : _rotatePivot;
            Vector3 dist = _introsActive ? _introDistance : _distance;
            _targetPosition.position = pivot + Quaternion.Euler(0f, Time.time * 20f, 0f) * dist;
            yield return null;
        }
    }
    
}
