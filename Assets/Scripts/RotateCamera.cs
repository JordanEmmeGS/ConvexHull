using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    private float speed = 5.0f;
 
    void Update() {
        if(Input.GetMouseButton(1))
        {
            float rotX = Input.GetAxis("Mouse X")*speed;
            float rotY = - Input.GetAxis("Mouse Y")*speed;
            transform.RotateAround(Vector3.zero, Vector3.up, rotX);
            transform.RotateAround(Vector3.zero, transform.right, rotY);
        }
    }
}