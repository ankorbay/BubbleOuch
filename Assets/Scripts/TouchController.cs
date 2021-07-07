using UnityEngine;

public class TouchController : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] LayerMask selectableLayer;
    
    Ray ray;
    RaycastHit hitData;
    GameObject firstTouchedGameObject;
    GameObject touchedOnMoveGameObject;
    public GameObject GetObjectTouched()
    {
        GameObject touchedObject = null;
        
        if (Input.touches.Length > 0)
        {
            ray = camera.ScreenPointToRay(Input.GetTouch(0).position);
            // ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitData))
            {
                touchedObject = hitData.transform.gameObject;
            }
        }

        return touchedObject;
    }

}