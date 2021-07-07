using System;
using DG.Tweening;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    Vector3 touchPosWorld;

    bool areNormalsFlipped = false;
    bool isActive = true;
    bool isAnimating = false;

    public bool IsActive
    {
        get => isActive;
        set => isActive = value;
    }

    public bool IsAnimating
    {
        get => isAnimating;
        private set => isAnimating = value;
    }


    void Update()
    {
        //TrackNormals();
    }

    void TrackNormals()
    {
        if (transform.localScale.z <= 0f && !areNormalsFlipped)
        {
            FlipNormals();
        }
        else if (transform.localScale.z > 0 && areNormalsFlipped)
        {
            FlipNormals();
        }
    }

    void FlipNormals()
    {
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;
        
        Vector3[] normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -1 * normals[i];
        }
        mesh.normals = normals;

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            int[] tris = mesh.GetTriangles(i);
            for(int j = 0; j < tris.Length; j += 3)
            {
                int temp = tris[j];
                tris[j] = tris[j + 1];
                tris[j + 1] = temp;
            }
            mesh.SetTriangles(tris, i);
        }

        areNormalsFlipped = !areNormalsFlipped;
    }

    
    public void Push(float duration)
    {
        if (!IsActive || IsAnimating)
        {
            Debug.Log("You can't push that bubble");
            return;
        }
        IsActive = false; // rename
        IsAnimating = true;
        
        float endValue;
        if (transform.localScale.z > 0f) // to const
        {
            endValue = -1f;
        }
        else
        {
            endValue = 1f;
        }

        transform.DOScaleZ(endValue, duration).OnComplete(() =>
        {
            IsAnimating = false;
        });
    }

    public void Reset()
    {
        IsActive = true;
    }
}
