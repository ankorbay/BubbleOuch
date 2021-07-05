using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BubbleController : MonoBehaviour
{
    [SerializeField] GameObject plane;
    [SerializeField] Transform pivotGameObject;
    [SerializeField] LayerMask selectableLayer;
    [SerializeField] private Camera camera;

    [SerializeField] private float pushRotationMultiplierCoeff = 1f;
    [SerializeField] private float centralBubblePushZShift = 0.1f;
    [SerializeField] private float bubblePushedAnimationTime = 0.4f;
    [SerializeField] private float restartAnimationDuration = 2f;

    bool isFinalAnimationFinished = false;
    GameObject selectedObject;
    Ray ray;
    RaycastHit hitData;
    int BubblesCount = 5;
    Bubble lastBubbleTouched;
    List<Bubble> bubbles;


    void Start()
    {
        bubbles = new List<Bubble>();
    }

    void Update()
    {
        //ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitData, 1000, selectableLayer))
        {
            selectedObject = hitData.transform.gameObject;
            Bubble bubble = selectedObject.GetComponent<Bubble>();

            if (bubble != null && bubble.IsActive && !bubble.IsAnimating)
            {
                bubble.Push();
                MovePlaneAfterShpereTouch(selectedObject);
                lastBubbleTouched = bubble;
                bubbles.Add(bubble);
            }
        }

        if (bubbles.Count == BubblesCount && !lastBubbleTouched.IsAnimating && !isFinalAnimationFinished)
        {
            PlayFinalAnimation();
        }
    }


    void PlayFinalAnimation()
    {
        plane.transform.RotateAround(pivotGameObject.position, Vector3.up, -100 * Time.deltaTime );
        if (plane.transform.eulerAngles.y <= 180f)
        {
            foreach (var bubble in bubbles)
            {
                bubble.gameObject.transform.localScale = new Vector3(100f, -40f, 50f);
            }
        } else if (plane.transform.rotation.y > 0f)
        {
            print("finished " + plane.transform.rotation.y);
            plane.transform.rotation = Quaternion.identity;
            isFinalAnimationFinished = true;
            ActivateBubbles();
        }
    }

    void ActivateBubbles()
    {
        foreach (var bubble in bubbles)
        {
            bubble.IsActive = true;
        }
        bubbles.Clear();
        isFinalAnimationFinished = false;
    }

    void MovePlaneAfterShpereTouch(GameObject selectedObject)
    {
        if (selectedObject.transform.localPosition.x == 0 && selectedObject.transform.localPosition.z == 0)
        {
            Sequence sequence = DOTween.Sequence();
            var position = plane.transform.position;
            sequence.Append(plane.transform.DOMoveZ(position.z + centralBubblePushZShift, bubblePushedAnimationTime/2f));
            sequence.Append(plane.transform.DOMoveZ(position.z, bubblePushedAnimationTime/2f));
        }
        else
        {
            var eulerAngles = plane.transform.eulerAngles;
            float currentXrotation = eulerAngles.x;
            float currentYrotation = eulerAngles.y;
            float currentZrotation = eulerAngles.z;

            Sequence sequence = DOTween.Sequence();
            var position = selectedObject.transform.position;
            sequence.Append(plane.transform.DORotate(
                new Vector3(currentXrotation + position.y * pushRotationMultiplierCoeff,
                            currentYrotation - position.x * pushRotationMultiplierCoeff,
                            currentZrotation),
                0.2f));
            sequence.Append(plane.transform.DORotate(
                new Vector3(currentXrotation,
                            currentYrotation,
                            currentZrotation),
                0.2f));
        }
    }
}
