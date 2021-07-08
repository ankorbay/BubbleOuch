using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Loader : MonoBehaviour
{
    [SerializeField] Transform rootObjectTransform;
    [SerializeField] PopitMeshes popitMeshes;

    private Vector3 defaultMeshRotation = new Vector3(-90, 0f, 0f); // TODO update after models fix
    Dictionary<int,GameObject> InstantiatedGameObjects;
    GameObject currentlyActiveGO;
    
    
    void Start()
    {
        InstantiatedGameObjects = new Dictionary<int, GameObject>();
        Load(0);

        // TestRuntimeSpawn();
    }

    
    void TestRuntimeSpawn()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.PrependInterval(4f);
        sequence.AppendCallback(() => { Load(1); });
        sequence.AppendInterval(4f);
        sequence.OnComplete(() => { Load(0); });
    }

    void Load(int index)
    {
        if (InstantiatedGameObjects.Count > 0)
        {
            currentlyActiveGO.gameObject.SetActive(false);
            
            print("InstantiatedGameObjects.Count="+InstantiatedGameObjects.Count);
            if (InstantiatedGameObjects.ContainsKey(index))
            {
                InstantiatedGameObjects[index].gameObject.SetActive(true);
            }
            else
            {
                SpawnEntity(index);
            }
        }
        else
        {
            SpawnEntity(index);
        }
    }

    void SpawnEntity(int index)
    {
        currentlyActiveGO = Instantiate(popitMeshes.prefabs[index], Vector3.zero, Quaternion.Euler(defaultMeshRotation));
        currentlyActiveGO.transform.parent = rootObjectTransform;
        InstantiatedGameObjects.Add(index,currentlyActiveGO);
    }
}
