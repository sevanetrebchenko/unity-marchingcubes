using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public GameObject target;
    public float speed;
    void Update()
    {
        transform.RotateAround(target.transform.position, Vector3.up, speed * Time.deltaTime);
    }
}
