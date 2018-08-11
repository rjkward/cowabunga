using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] private Rigidbody _atomPrefab;

    private Rigidbody[][] _floor;
    private const int AtomsPerSide = 25;
    private const float AtomLength = 2f;

    private void Awake()
    {
        _floor = new Rigidbody[AtomsPerSide][];
        for (int i = 0; i < _floor.Length; i++)
        {
            _floor[i] = new Rigidbody[AtomsPerSide];
        }
    }

    private void Start ()
    {
        GenerateFloor();
        StartCoroutine(Erode());
    }

    private void GenerateFloor()
    {
        for (int i = 0; i < AtomsPerSide; i++)
        {
            for (int j = 0; j < AtomsPerSide; j++)
            {
                Rigidbody newAtom = Instantiate(_atomPrefab, transform);
                _floor[i][j] = newAtom;
                newAtom.position = new Vector3(i * AtomLength, 0f, j * AtomLength);
            }
        }
    }

    // private void Cleanup()
    // {
    //     StopAllCoroutines();
    //     for (int i = 0; i < _floor.Count; i++)
    //     {
    //         if (_floor[i] != null)
    //         {
    //             Destroy(_floor[i].gameObject);
    //         }
    //     }
    //     
    //     _floorAtoms.Clear();
    // }
    
    private readonly WaitForFixedUpdate _waitForFixed = new WaitForFixedUpdate();
    private readonly WaitForSeconds _waitForOne = new WaitForSeconds(1f);
    private const float EdgeFallChance = 0.2f;
    private const float InnerFallChance = 0.001f;

    private IEnumerator Erode()
    {
        while (true)
        {
            for (int i = 0; i < _floor.Length; i++)
            {
                Rigidbody[] row = _floor[i];
                for (int j = 0; j < row.Length; j++)
                {
                    yield return _waitForFixed;
                    Rigidbody atom = row[j];
                    if (!atom.isKinematic)
                    {
                        continue;
                    }

                    int openEdges = GetOpenEdges(i, j);
                    if (openEdges > 0 &&
                        (openEdges > 2 || Random.value * openEdges < EdgeFallChance))
                    {
                        atom.isKinematic = false;
                        continue;
                    }

                    if (Random.value < InnerFallChance)
                    {
                        atom.isKinematic = false;
                    }
                }
            }
        }
    }

    private int GetOpenEdges(int i, int j)
    {
        int openSides = 0;
        if (j + 1 == AtomsPerSide || !_floor[i][j + 1].isKinematic)
        {
            openSides += 1;
        }
        
        if (j == 0 || !_floor[i][j - 1].isKinematic)
        {
            openSides += 1;
        }
        
        if (i + 1 == AtomsPerSide || !_floor[i + 1][j].isKinematic)
        {
            openSides += 1;
        }
        
        if (i == 0 || !_floor[i - 1][j].isKinematic)
        {
            openSides += 1;
        }

        return openSides;
    }
}
