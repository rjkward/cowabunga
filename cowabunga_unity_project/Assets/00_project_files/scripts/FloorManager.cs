using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] private Rigidbody _atomPrefab;

    private Rigidbody[][] _floor;
    private bool[][] _floorModel;
    private const int AtomsPerSide = 25;
    private const float AtomLength = 2f;

    private void Awake()
    {
        _floor = new Rigidbody[AtomsPerSide][];
        _floorModel = new bool[AtomsPerSide][];
        for (int i = 0; i < _floor.Length; i++)
        {
            _floor[i] = new Rigidbody[AtomsPerSide];
            _floorModel[i] = new bool[AtomsPerSide];
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
    private const float EdgeFallChance = 0.01f;
    private const float InnerFallChance = 0.0001f;

    private readonly List<Vector2> _fallBuffer = new List<Vector2>();

    private IEnumerator Erode()
    {
        while (true)
        {
            for (int i = 0; i < AtomsPerSide; i++)
            {
                bool[] row = _floorModel[i];
                for (int j = 0; j < AtomsPerSide; j++)
                {
                    bool eroded = row[j];
                    if (eroded)
                    {
                        continue;
                    }

                    int openEdges = GetOpenEdges(i, j);
                    if (openEdges > 0 &&
                        (openEdges > 2 || Random.value * openEdges < EdgeFallChance))
                    {
                        _fallBuffer.Add(new Vector2(i, j));
                        continue;
                    }

                    if (Random.value < InnerFallChance)
                    {
                        _fallBuffer.Add(new Vector2(i, j));
                    }
                }
                yield return _waitForFixed;
            }
            
            _fallBuffer.ForEach(vector2 =>
            {
                int i = (int)vector2.x;
                int j = (int)vector2.y;
                _floor[i][j].isKinematic = false;
                _floorModel[i][j] = true;
            });
            
            _fallBuffer.Clear();
        }
    }

    private int GetOpenEdges(int i, int j)
    {
        int openSides = 0;
        if (j + 1 == AtomsPerSide || _floorModel[i][j + 1])
        {
            openSides += 1;
        }
        
        if (j == 0 || _floorModel[i][j - 1])
        {
            openSides += 1;
        }
        
        if (i + 1 == AtomsPerSide || _floorModel[i + 1][j])
        {
            openSides += 1;
        }
        
        if (i == 0 || _floorModel[i - 1][j])
        {
            openSides += 1;
        }

        return openSides;
    }
}
