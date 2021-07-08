using System;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    [SerializeField] Transform rootObjectTransform;
    
    [SerializeField] PopitMeshes popitMeshes;

    Dictionary<int,GameObject> InstantiatedGameObjects;
    GameObject currentlyActiveGO;
    void Start()
    {
        InstantiatedGameObjects = new Dictionary<int, GameObject>();
        Load(1);
    }

    void Load(int index)
    {
        if (InstantiatedGameObjects.Count > 0)
        {
            if (InstantiatedGameObjects.ContainsKey(index))
            {
                currentlyActiveGO.SetActive(false);
                InstantiatedGameObjects[index].gameObject.SetActive(true);
            }
        }
        else
        {
            currentlyActiveGO = Instantiate(popitMeshes.prefabs[index], Vector3.zero, Quaternion.identity);
            currentlyActiveGO.transform.parent = rootObjectTransform;
            InstantiatedGameObjects.Add(index,currentlyActiveGO);
        }
        
    }
}
