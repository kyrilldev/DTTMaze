using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [Header("Zoom Variables")]
    [SerializeField] private float _minZoomAmount;
    [SerializeField] private float _maxZoomAmount;
    [SerializeField] private float _startZoomAmount;

    [Header("Vector3 Touch")]
    [SerializeField] private Vector3 _touchstart;

    private void Update()
    {
        var zoom = Input.GetAxis("Mouse ScrollWheel");
        Zoom(zoom);

        //on first touch
        if (Input.GetMouseButtonDown(0))
        {
            _touchstart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        //multiple touches
        if (Input.touchCount == 2)
        {
            //get first and second touch
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            //previous touch pos from current position - the difference between current touch and prev touch (deltaPosition)
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            
            //prev vector length
            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            //current vector length
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            //length off the difference between 2 vectors
            float difference = currentMagnitude - prevMagnitude;
            //apply
            Zoom(difference * 0.01f);
        }
        //when user keeps holding
        else if (Input.GetMouseButton(0))
        {
            //calculate move amount by subtracting ,mouse position on screen, off touch start set before
            Vector3 direction = _touchstart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //apply new position
            Camera.main.transform.position += direction;
        }
    }

    private void Zoom(float increment)
    {
        //apply size, clamped with pre set min and max
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, _minZoomAmount, _maxZoomAmount);
    }
}
