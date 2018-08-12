using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using Random = UnityEngine.Random;

public class FloorManager : MonoBehaviour
{
    private struct IntPair
    {
        public readonly int I;
        public readonly int J;
        public IntPair(int i, int j)
        {
            I = i;
            J = j;
        }
    }
    
    private const int AtomsPerSide = 25;
    private const float AtomLength = 2f;
    private const float EdgeFallChance = 0.05f;
    private const float InnerFallChance = 0.00005f;
    private const float DisableDepth = -50f;
    private readonly Rigidbody[][] _floorAtoms = new Rigidbody[AtomsPerSide][];
    private readonly AtomState[][] _floorState = new AtomState[AtomsPerSide][];
    private readonly List<IntPair> _fallBuffer = new List<IntPair>();
    private readonly WaitForSeconds _wait = new WaitForSeconds(1f / 60f);
    [SerializeField]
    private Rigidbody _atomPrefab;
    public bool FloorLoaded;

    private void Awake()
    {
        for (int i = 0; i < AtomsPerSide; i++)
        {
            _floorAtoms[i] = new Rigidbody[AtomsPerSide];
            _floorState[i] = new AtomState[AtomsPerSide];
        }
    }

    private void Start()
    {
        GenerateFloor();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void GenerateFloor()
    {
        for (int i = 0; i < AtomsPerSide; i++)
        {
            for (int j = 0; j < AtomsPerSide; j++)
            {
                Rigidbody newAtom = Instantiate(_atomPrefab, transform);
                _floorAtoms[i][j] = newAtom;
                newAtom.position = new Vector3(i * AtomLength, 0f, j * AtomLength);
            }
        }

        FloorLoaded = true;
    }

    public void StartErosion()
    {
        StartCoroutine(Erode());
    }

    private IEnumerator Erode()
    {
        while (true) // aaaaaagh!!!
        {
            bool noActiveOrFallingAtoms = true;
            for (int i = 0; i < AtomsPerSide; i++)
            {
                AtomState[] row = _floorState[i];
                for (int j = 0; j < AtomsPerSide; j++)
                {
                    AtomState atomState = row[j];
                    switch (atomState)
                    {
                        case AtomState.Disabled:
                            break;
                        case AtomState.Active:
                            int fallenNeighbours = GetFallenNeighbourCount(i, j);
                            if (fallenNeighbours > 0)
                            {
                                if (fallenNeighbours > 2 || Random.value * fallenNeighbours < EdgeFallChance)
                                {
                                    _fallBuffer.Add(new IntPair(i, j));
                                }
                            }
                            else if (Random.value < InnerFallChance)
                            {
                                _fallBuffer.Add(new IntPair(i, j));
                            }

                            noActiveOrFallingAtoms = false;
                            break;
                        case AtomState.Falling:
                            Rigidbody atom = _floorAtoms[i][j];
                            if (atom.transform.position.y < DisableDepth)
                            {
                                atom.gameObject.SetActive(false);
                                row[j] = AtomState.Disabled;
                            }

                            noActiveOrFallingAtoms = false;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                
                yield return _wait;
            }

            int count = _fallBuffer.Count;
            for (int x = 0; x < count; x++)
            {
                IntPair atomPos = _fallBuffer[x];
                int i = atomPos.I;
                int j = atomPos.J;
                _floorAtoms[i][j].isKinematic = false;
                _floorState[i][j] = AtomState.Falling;
            }
            
            _fallBuffer.Clear();
            if (noActiveOrFallingAtoms)
            {
                break;
            }
        }
    }

    private int GetFallenNeighbourCount(int i, int j)
    {
        int fallen = 0;
        if (j + 1 == AtomsPerSide || _floorState[i][j + 1] != AtomState.Active)
        {
            fallen += 1;
        }
        
        if (j == 0 || _floorState[i][j - 1] != AtomState.Active)
        {
            fallen += 1;
        }
        
        if (i + 1 == AtomsPerSide || _floorState[i + 1][j] != AtomState.Active)
        {
            fallen += 1;
        }
        
        if (i == 0 || _floorState[i - 1][j] != AtomState.Active)
        {
            fallen += 1;
        }

        return fallen;
    }
}
