using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class SelectOnClick : MonoBehaviour
{
    [SerializeField] GameObject plane;

    public GameObject selectedObject;
    public LayerMask selectableLayer;

    Ray ray;
    RaycastHit hitData;
    int PopItElementsCount = 5;
    PopItElement lastElementTouched;
    List <PopItElement> popItElements;


    void Start()
    {
        popItElements = new List<PopItElement>();
    }

    void Update()
    {
        //ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitData, 1000, selectableLayer))
        {
            selectedObject = hitData.transform.gameObject;
            PopItElement shpere = selectedObject.GetComponent<PopItElement>();

            if (shpere != null && shpere.IsActive && !shpere.IsAnimating)
            {
                shpere.Push();
                MovePlaneAfterShpereTouch(selectedObject);
                lastElementTouched = shpere;
                popItElements.Add(shpere);
            }
        }

        if (popItElements.Count == PopItElementsCount && !lastElementTouched.IsActive)
        {
            RestartLevel();
        }
    }


    void RestartLevel()
    {
        foreach (var element in popItElements)
        {
            element.gameObject.transform.DOScaleY(-40f, 1f).OnComplete(() => popItElements.Remove(element));
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
                new Vector3(currentXrotation + selectedObject.transform.position.y,
                            currentYrotation - selectedObject.transform.position.x,
                            currentZrotation ),
                0.2f));
            sequence.Append(plane.transform.DORotate(
                new Vector3(currentXrotation,
                            currentYrotation,
                            currentZrotation),
                0.2f));
        }
    }
}