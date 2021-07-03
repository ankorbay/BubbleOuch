using System;
using DG.Tweening;
using UnityEngine;

public enum eBaloonState
{
    FULL,
    BLOWS_OFF,
    EMPTY
}

public class PopItElement : MonoBehaviour
{
    [SerializeField] [Range(-40f, 20f)] float zValue = -40f;

    Vector3 touchPosWorld;
    bool areNormalsFlipped = false;

    TouchPhase touchPhase = TouchPhase.Ended;

    void Start()
    {
        transform.localScale = new Vector3(transform.localScale.x, zValue, transform.localScale.z);
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == touchPhase)
        {
            touchPosWorld = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

            Vector2 touchPosWorld2D = new Vector2(touchPosWorld.x, touchPosWorld.y);

            RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);

            if (hitInformation.collider != null)
            {
                GameObject touchedObject = hitInformation.transform.gameObject;
                Debug.Log("Touched " + touchedObject.transform.name);
                touchedObject.transform.DOScaleY(20f, 1f);
            }
        }

        if (transform.localScale.y >= 0f && !areNormalsFlipped) FlipNormals();
    }

    void FlipNormals()
    {
        print("flip");
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

        areNormalsFlipped = true;
    }

    public void Push()
    {
        transform.DOScaleY(20f, 1f);
    }
}
