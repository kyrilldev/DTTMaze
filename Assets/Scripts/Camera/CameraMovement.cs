using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Vector3 camPos;
    [SerializeField] private float moveAmount;
    private Camera m_Camera;
    private float divideAmount = 24;

    private void Awake()
    {
        m_Camera = GetComponent<Camera>();
    }
    private void Start()
    {
        camPos.y = m_Camera.transform.position.y;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            CamMoving();
        }
    }

    private void FixedUpdate()
    {
        MoveAmountScaling();
            
        m_Camera.transform.position = Vector3.Lerp(m_Camera.transform.position, camPos, Time.deltaTime * 8);
    }

    private void CamMoving()
    {
        if (Input.GetAxis("Mouse X") > 0)
        {
            camPos.x += moveAmount;
        }
        if (Input.GetAxis("Mouse X") < 0)
        {
            camPos.x -= moveAmount;
        }

        if (Input.GetAxis("Mouse Y") > 0)
        {
            camPos.z += moveAmount;
        }
        if (Input.GetAxis("Mouse Y") < 0)
        {
            camPos.z -= moveAmount;
        }
    }

    private float MoveAmountScaling()
    {
        return moveAmount = (m_Camera.orthographicSize / divideAmount) * 0.5f;
    }
}
