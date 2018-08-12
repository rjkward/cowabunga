using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static event Action<InputManager> NewInput;
    public readonly List<KeyCode> KeyDownList = new List<KeyCode>();
    public readonly HashSet<KeyCode> KeyDownHashes = new HashSet<KeyCode>();
    public readonly List<KeyCode> KeyUpList = new List<KeyCode>();
    public readonly HashSet<KeyCode> KeyUpHashes = new HashSet<KeyCode>();
    public bool Space { get; private set; }
    private readonly KeyCode[] _alphabet = new KeyCode[26];

    private void Awake()
    {
        _alphabet[0] = KeyCode.A;
        _alphabet[1] = KeyCode.B;
        _alphabet[2] = KeyCode.C;
        _alphabet[3] = KeyCode.D;
        _alphabet[4] = KeyCode.E;
        _alphabet[5] = KeyCode.F;
        _alphabet[6] = KeyCode.G;
        _alphabet[7] = KeyCode.H;
        _alphabet[8] = KeyCode.I;
        _alphabet[9] = KeyCode.J;
        _alphabet[10] = KeyCode.K;
        _alphabet[11] = KeyCode.L;
        _alphabet[12] = KeyCode.M;
        _alphabet[13] = KeyCode.N;
        _alphabet[14] = KeyCode.O;
        _alphabet[15] = KeyCode.P;
        _alphabet[16] = KeyCode.Q;
        _alphabet[17] = KeyCode.R;
        _alphabet[18] = KeyCode.S;
        _alphabet[19] = KeyCode.T;
        _alphabet[20] = KeyCode.U;
        _alphabet[21] = KeyCode.V;
        _alphabet[22] = KeyCode.W;
        _alphabet[23] = KeyCode.X;
        _alphabet[24] = KeyCode.Y;
        _alphabet[25] = KeyCode.Z;
    }

    private void Update()
    {
        KeyDownList.Clear();
        KeyDownHashes.Clear();
        KeyUpList.Clear();
        KeyUpHashes.Clear();
        Space = false;
        
        for (int i = 0; i < 26; i++)
        {
            KeyCode key = _alphabet[i];
            if (Input.GetKeyDown(key))
            {
                KeyDownList.Add(key);
                KeyDownHashes.Add(key);
            }
            
            if (Input.GetKeyUp(key))
            {
                KeyUpList.Add(key);
                KeyUpHashes.Add(key);
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            Space = true;
        }

        if (NewInput != null)
        {
            NewInput.Invoke(this);
        }
    }
}