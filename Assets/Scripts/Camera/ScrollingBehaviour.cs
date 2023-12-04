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

    private void Start()
    {
        cam = GetComponent<Camera>();
        zoomAmount = startZoomAmount;
        previousSize = cam.orthographicSize;
    }

    private void LateUpdate()
    {
        float scrollDelta = Input.mouseScrollDelta.y;
        var newZoom = zoomAmount += (scrollDelta * ZoomMultiplier());
        newZoom = Mathf.Clamp(newZoom, minZoomAmount, maxZoomAmount);
        cam.orthographicSize = Mathf.Lerp(previousSize, newZoom, Time.deltaTime * 8);
        zoomAmount = newZoom;
        previousSize = zoomAmount;
    }

    private float ZoomMultiplier()
    {
        return zoomAmount / 20;
    }
}
