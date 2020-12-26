using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraRotate : MonoBehaviour
{
    private Vector3 _previousMousePosition;
    private float _cutoffAngle;
    private GameObject _target;

    private void Start()
    {
        _target = GameObject.Find("CameraFocus");
        _previousMousePosition = Input.mousePosition;
        _cutoffAngle = 80.0f;
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 targetPosition = _target.transform.position;
            
            // Rotate horizontally.
            transform.RotateAround(targetPosition, Vector3.up, (currentMousePosition.x - _previousMousePosition.x) / 5.0f);
            
            // Rotate vertically.
            Vector3 axis = Vector3.Cross(transform.forward, Vector3.up);
            transform.RotateAround(targetPosition, axis, (currentMousePosition.y - _previousMousePosition.y) / 5.0f);
            
            // If the transform rotation exceeded the boundary, set it back.
            // Looking down at object.
            if (transform.eulerAngles.x > _cutoffAngle && transform.eulerAngles.x < 90.0f)
            {
                float angle = Mathf.Abs(transform.eulerAngles.x - _cutoffAngle);
                transform.RotateAround(targetPosition, axis, angle);
            }
            // Looking up at object.
            else if (transform.eulerAngles.x < 360.0f - _cutoffAngle && transform.eulerAngles.x > 270.0f)
            {
                float angle = -Mathf.Abs((360.0f - _cutoffAngle) - transform.eulerAngles.x);
                transform.RotateAround(targetPosition, axis, angle);
            }
        }
        
        _previousMousePosition = Input.mousePosition;
        // transform.RotateAround(target.transform.position, Vector3.up, speed * Time.deltaTime);
    }
}
