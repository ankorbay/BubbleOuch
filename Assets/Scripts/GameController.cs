using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameController : MonoBehaviour
{
    private const float ROOT_OBJECT_ROTATION_ON_FINISH = 180f;
    
    [SerializeField] TouchController touchController;
    
    [SerializeField] Transform rootTransform;
    [SerializeField] Transform rootChildTransform;
    [SerializeField] Transform pivotTransform;
    [SerializeField] GameObject selectedObject;

    [SerializeField] float rootRotationCoeff = 1f;
    [SerializeField] float rootPushAnimationTime = 0.4f;
    [SerializeField] float bubbleAnimationTime = 1f;
    [SerializeField] float rootRestartAnimationTime = 3f;

    [SerializeField] private List<Bubble> bubblesInstantiated;
    
    bool isPlaneAnimationActive = false;
    bool isPlaneFlipped = false;
    Ray ray;
    RaycastHit hitData;
    int bubblesInstantiatedCount;
    GameObject previousObjectTouched = null;
    Bubble currentBubbleTouched;
    List<Bubble> bubblesTouched;
    float startPlaneRotation;
    float endPlaneRotation;

    void Start()
    {
        bubblesTouched = new List<Bubble>();
        bubblesInstantiatedCount = bubblesInstantiated.Count;
        
        var rootPosition = rootTransform.position;
        var pivotPosition = pivotTransform.position;
        rootPosition = new Vector3(rootPosition.x - pivotPosition.x,0f,0f);
        
        rootTransform.position = rootPosition;
        rootChildTransform.position = new Vector3(rootPosition.x + pivotPosition.x,0f,0f);
    }

    void Update()
    {
        TrackBubbleCollisions();

        if (bubblesTouched.Count == bubblesInstantiatedCount && !currentBubbleTouched.IsAnimating && !isPlaneAnimationActive)
        {
            RunPlaneAnimation();
        }
    }

    void TrackBubbleCollisions()
    {
        selectedObject = touchController.GetObjectTouched();

            if (selectedObject != null && selectedObject != previousObjectTouched)
            {
                currentBubbleTouched = selectedObject.GetComponent<Bubble>();
                if (currentBubbleTouched != null)
                {
                    AnimateRootOnTouch(selectedObject);
                    
                    if (currentBubbleTouched.IsActive)
                    {
                        currentBubbleTouched.Push(bubbleAnimationTime);
                        bubblesTouched.Add(currentBubbleTouched);
                    }
                }
            } else if (selectedObject == null) previousObjectTouched = null;
    }

    void RunPlaneAnimation()
    {
        isPlaneAnimationActive = true;
        startPlaneRotation = rootTransform.eulerAngles.y;
        endPlaneRotation = startPlaneRotation - ROOT_OBJECT_ROTATION_ON_FINISH;
        if (isPlaneFlipped) endPlaneRotation = startPlaneRotation + ROOT_OBJECT_ROTATION_ON_FINISH;
        rootTransform.DORotate(new Vector3(0f, endPlaneRotation, 0f), rootRestartAnimationTime)
            .OnComplete(() =>
            {
                isPlaneFlipped = !isPlaneFlipped;
                isPlaneAnimationActive = false;
                previousObjectTouched = null;
                ActivateBubbles();
            }); // extract
    }

    void ActivateBubbles()
    {
        foreach (var bubble in bubblesTouched)
        {
            bubble.Reset();
        }
        bubblesTouched.Clear();
        selectedObject = null;
    }

    void AnimateRootOnTouch(GameObject selectedObject) // call always on touch
    {
        previousObjectTouched = selectedObject;
        if (isPlaneAnimationActive == true) return;
        isPlaneAnimationActive = true;
        var position = selectedObject.transform.position;
        {
            var startEulerAngles = rootTransform.eulerAngles;

            float targetXrotation = position.y * rootRotationCoeff;
            float targetYrotation = - position.x * rootRotationCoeff;
            if (isPlaneFlipped)
            {
                targetXrotation = - position.y * rootRotationCoeff;
                targetYrotation = 180f - position.x * rootRotationCoeff;
            }
            Vector3 targetEulerAngles = new Vector3(targetXrotation, targetYrotation, startEulerAngles.z);
            
            Sequence sequence = DOTween.Sequence();
            sequence.Append(rootTransform.DORotate(
                targetEulerAngles,
                rootPushAnimationTime/2f));
            sequence.Append(rootTransform.DORotate(
                startEulerAngles,
                rootPushAnimationTime/2f));
            sequence.OnComplete(() => isPlaneAnimationActive = false);
        }
    }
}
