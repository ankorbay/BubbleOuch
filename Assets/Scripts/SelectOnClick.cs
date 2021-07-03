using UnityEngine;
using DG.Tweening;

public class SelectOnClick : MonoBehaviour
{
    [SerializeField] GameObject plane;

    public GameObject selectedObject;
    public GameObject highlightedObject;
    public LayerMask selectableLayer;
    Ray ray;
    RaycastHit hitData;

    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitData, 1000, selectableLayer))
        {
            highlightedObject = hitData.transform.gameObject;

            if (Input.GetMouseButtonDown(0))
            {
                selectedObject = hitData.transform.gameObject;

                PopItElement shpere = selectedObject.GetComponent<PopItElement>();

                if (shpere != null)
                {
                    shpere.Push();
                    MovePlaneAfterShpereTouch(selectedObject);
                }
            }
        }
        else
        {
            highlightedObject = null;

            if (Input.GetMouseButtonDown(0))
            {
                selectedObject = null;
            }
        }
    }

    void MovePlaneAfterShpereTouch(GameObject selectedObject)
    {
        if(selectedObject.transform.localPosition.x == 0 && selectedObject.transform.localPosition.z == 0)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(plane.transform.DOMoveZ(plane.transform.position.z + 0.1f, 0.2f));
            sequence.Append(plane.transform.DOMoveZ(plane.transform.position.z, 0.2f));
        }
        else
        {
            float currentXrotation = plane.transform.eulerAngles.x;
            float currentYrotation = plane.transform.eulerAngles.y;
            float currentZrotation = plane.transform.eulerAngles.z;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(plane.transform.DORotate(
                new Vector3(currentXrotation - selectedObject.transform.position.z,
                            currentYrotation,
                            currentZrotation - selectedObject.transform.position.x),
                0.2f));
            sequence.Append(plane.transform.DORotate(
                new Vector3(currentXrotation,
                            currentYrotation,
                            currentZrotation),
                0.2f));
        }
    }
}