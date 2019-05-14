using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIndicator : MonoBehaviour
{
    [SerializeField]
    private Text _text;

    private Rob_CharacterController _player;

    private Camera _camera;

    private KeyCode _key;

    private void OnEnable()
    {
        InputManager.NewInput += HandleNewInput;
    }

    private void OnDisable()
    {
        InputManager.NewInput -= HandleNewInput;
        _text.enabled = false;
    }

    private void HandleNewInput(InputManager inputManager)
    {
        if (inputManager.KeyDownHashes.Contains(_key))
        {
            _text.enabled = true;
        }
        else if (inputManager.KeyUpHashes.Contains(_key))
        {
            _text.enabled = false;
        }
    }

    public void Init(KeyCode key, Rob_CharacterController player, Camera camera)
    {
        _key = key;
        _text.text = key.ToString();
        _player = player;
        _camera = camera;
        LateUpdate();
    }

    private void LateUpdate()
    {
        transform.position = _camera.WorldToScreenPoint(_player.transform.position) - 40f * Vector3.up;
    }
}
