using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    private struct BoolPair
    {
        public bool Falling;
        public bool Disabled;
    }
    
    private const int AtomsPerSide = 25;
    private const float AtomLength = 2f;
    private const float EdgeFallChance = 0.05f;
    private const float InnerFallChance = 0.00005f;
    private const float DisableDepth = -50f;
    private readonly Rigidbody[][] _floorView = new Rigidbody[AtomsPerSide][];
    private readonly BoolPair[][] _floorModel = new BoolPair[AtomsPerSide][];
    private readonly List<IntPair> _fallBuffer = new List<IntPair>();
    private readonly WaitForSeconds _wait = new WaitForSeconds(1f / 60f);
    [SerializeField]
    private Rigidbody _atomPrefab;

    private void Awake()
    {
        for (int i = 0; i < AtomsPerSide; i++)
        {
            _floorView[i] = new Rigidbody[AtomsPerSide];
            _floorModel[i] = new BoolPair[AtomsPerSide];
        }
    }

    private void Start()
    {
        GenerateFloor();
        StartCoroutine(Erode());
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
                _floorView[i][j] = newAtom;
                newAtom.position = new Vector3(i * AtomLength, 0f, j * AtomLength);
            }
        }
    }
    


    private IEnumerator Erode()
    {
        while (true) // aaaaaagh!!!
        {
            bool finished = true;
            for (int i = 0; i < AtomsPerSide; i++)
            {
                BoolPair[] row = _floorModel[i];
                for (int j = 0; j < AtomsPerSide; j++)
                {
                    BoolPair atomState = row[j];
                    if (atomState.Falling)
                    {
                        if (!atomState.Disabled)
                        {
                            finished = false;
                            Rigidbody atom = _floorView[i][j];
                            if (atom.transform.position.y < DisableDepth)
                            {
                                atom.gameObject.SetActive(false);
                                atomState.Disabled = true;
                            }
                        }
                        
                        continue;
                    }
                    
                    finished = false;
                    int fallenNeighbours = GetFallenNeighbourCount(i, j);
                    if (fallenNeighbours > 0 &&
                        (fallenNeighbours > 2 || Random.value * fallenNeighbours < EdgeFallChance))
                    {
                        _fallBuffer.Add(new IntPair(i, j));
                        continue;
                    }

                    if (Random.value < InnerFallChance)
                    {
                        _fallBuffer.Add(new IntPair(i, j));
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
                _floorView[i][j].isKinematic = false;
                _floorModel[i][j].Falling = true;
            }
            
            _fallBuffer.Clear();
            if (finished)
            {
                Debug.LogError("done");
                yield break;
            }
        }
    }

    private int GetFallenNeighbourCount(int i, int j)
    {
        int fallen = 0;
        if (j + 1 == AtomsPerSide || _floorModel[i][j + 1].Falling)
        {
            fallen += 1;
        }
        
        if (j == 0 || _floorModel[i][j - 1].Falling)
        {
            fallen += 1;
        }
        
        if (i + 1 == AtomsPerSide || _floorModel[i + 1][j].Falling)
        {
            fallen += 1;
        }
        
        if (i == 0 || _floorModel[i - 1][j].Falling)
        {
            fallen += 1;
        }

        return fallen;
    }
}
