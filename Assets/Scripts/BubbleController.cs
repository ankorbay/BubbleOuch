using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BubbleController : MonoBehaviour
{
    [SerializeField] Transform plane;
    [SerializeField] Transform pivotGameObject;
    [SerializeField] GameObject selectedObject;
    [SerializeField] LayerMask selectableLayer;
    [SerializeField] private Camera camera;

    [SerializeField] private float rotationScale = 1f;
    [SerializeField] private float zShiftOnPush = 0.1f;
    [SerializeField] private float bubbleAnimationTime = 0.4f;
    [SerializeField] private float planeAnimationTime = 3f;

    bool isPlaneAnimationActive = false;
    bool isPlaneFlipped = false;
    Ray ray;
    RaycastHit hitData;
    int BubblesCount = 4;
    Bubble lastBubbleTouched;
    List<Bubble> bubbles;
    float startPlaneRotation;
    float endPlaneRotation;


    void Start()
    {
        bubbles = new List<Bubble>();
        plane.position = new Vector3(plane.position.x - pivotGameObject.position.x,0f,0f);
    }

    void Update()
    {
        TrackBubbleCollisions();

        if (bubbles.Count == BubblesCount && !lastBubbleTouched.IsAnimating && !isPlaneAnimationActive)
        {
            RunPlaneAnimation();
        } else if (isPlaneAnimationActive)
        {
            TrackPlaneRotation();
        }
    }

    void TrackBubbleCollisions()
    {
        if (!isPlaneAnimationActive)
        {
            if (Input.touches.Length > 0)
            {
                ray = camera.ScreenPointToRay(Input.GetTouch(0).position);
                //ray = camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hitData, 100, selectableLayer))
                {
                    selectedObject = hitData.transform.gameObject;
                    Bubble bubble = selectedObject.GetComponent<Bubble>();

                    if (bubble != null && bubble.IsActive && !bubble.IsAnimating)
                    {
                        bubble.Push();
                        MovePlaneAfterShpereTouch(selectedObject);
                        lastBubbleTouched = bubble;
                        bubbles.Add(bubble);
                        Debug.Log(bubbles.Count);
                    }
                }
            }
        }
    }


    void RunPlaneAnimation()
    {
        isPlaneAnimationActive = true;
        startPlaneRotation = plane.eulerAngles.y;
        endPlaneRotation = startPlaneRotation - 180f;
        plane.DORotate(new Vector3(0f, endPlaneRotation, 0f), planeAnimationTime)
            .OnComplete(() => isPlaneFlipped = !isPlaneFlipped);
    }

    void TrackPlaneRotation()
    {   
        print("tracking " + plane.eulerAngles.y);
        if (plane.eulerAngles.y <= 181f)
        {
            print("stopped at " + plane.eulerAngles.y);
            plane.eulerAngles = new Vector3(0f,180f,0f);
            ActivateBubbles();
            isPlaneAnimationActive = false;
        }
    }

    void ActivateBubbles()
    {
        foreach (var bubble in bubbles)
        {
            bubble.IsActive = true;
        }
        bubbles.Clear();
        selectedObject = null;
    }

    void MovePlaneAfterShpereTouch(GameObject selectedObject)
    {
        var position = selectedObject.transform.position;
        
        if (selectedObject.transform.localPosition.x == 0 && selectedObject.transform.localPosition.z == 0)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(plane.DOMoveZ(position.z + zShiftOnPush, bubbleAnimationTime/2f));
            sequence.Append(plane.DOMoveZ(position.z, bubbleAnimationTime/2f));
        }
        else
        {
            var eulerAngles = plane.eulerAngles;
            
            float currentXrotation = eulerAngles.x;
            float currentYrotation = eulerAngles.y;
            float currentZrotation = eulerAngles.z;

            float targetXrotation = currentXrotation + position.y * rotationScale;
            float targetYrotation = currentYrotation - position.x * rotationScale;
            if(isPlaneFlipped) targetXrotation = currentXrotation - position.y * rotationScale;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(plane.DORotate(
                new Vector3(targetXrotation,
                                    targetYrotation,
                            currentZrotation),
                0.2f));
            sequence.Append(plane.DORotate(
                new Vector3(currentXrotation,
                            currentYrotation,
                            currentZrotation),
                0.2f));
        }
    }
}
