using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScrollingBehaviour : MonoBehaviour
{
    [SerializeField] private float zoomAmount;
    [SerializeField] private float minZoomAmount;
    [SerializeField] private float maxZoomAmount;
    [SerializeField] private float startZoomAmount;
    private float previousSize;

    [SerializeField] private Camera cam;
    [SerializeField] Vector3 touchstart;

    private void Start()
    {
        cam = GetComponent<Camera>();
        zoomAmount = startZoomAmount;
        previousSize = cam.orthographicSize;
    }

    private void Update()
    {
        var zoom = Input.GetAxis("Mouse ScrollWheel");
        Debug.Log(zoom * ZoomMultiplier());
        Zoom(zoom);

        if (Input.GetMouseButtonDown(0))
        {
            touchstart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            Zoom(difference * 0.01f);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 direction = touchstart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Camera.main.transform.position += direction;
        }
    }

    private void Zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, minZoomAmount, maxZoomAmount);
    }

    private float ZoomMultiplier()
    {
        float size = Camera.main.orthographicSize / 20;
        return size * 30;
    }
}
