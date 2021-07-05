using DG.Tweening;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    float zValue = -40f;
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


    void Start()
    {
        var transform1 = transform;
        var localScale = transform1.localScale;
        localScale = new Vector3(localScale.x, zValue, localScale.z);
        transform1.localScale = localScale;
    }

    void Update()
    {
        if (transform.localScale.y >= 0f && !areNormalsFlipped)
        {
            FlipNormals();
        } else if (transform.localScale.y < 0 && areNormalsFlipped)
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


    public void Push()
    {
        IsActive = false;
        IsAnimating = true;
        transform.DOScaleY(20f, 1f).OnComplete(() =>
        {
            IsAnimating = false;
        });
    }
}
