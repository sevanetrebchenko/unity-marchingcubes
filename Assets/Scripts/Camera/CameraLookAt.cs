using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAt : MonoBehaviour
{
    private Transform _target;

    private void Start()
    {
        _target = GameObject.Find("CameraFocus").transform;
    }

    private void Update()
    {
        transform.LookAt(_target);
    }
}
