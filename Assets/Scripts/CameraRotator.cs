using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    private Quaternion originRotation;
    public float sensitivity = 3;
    private float angleHorizontal;
    private float angleVertical;


    // Start is called before the first frame update
    void Start()
    {
        originRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        angleHorizontal += Input.GetAxis("Mouse X") * sensitivity;
        angleVertical += -Input.GetAxis("Mouse Y") * sensitivity;

        angleVertical = Mathf.Clamp(angleVertical, -60, 60);
        angleHorizontal = Mathf.Clamp(angleHorizontal, -60, 60);

        Quaternion rotationY = Quaternion.AngleAxis(angleHorizontal, Vector3.up);
        Quaternion rotationX = Quaternion.AngleAxis(angleVertical, Vector3.right);

        transform.rotation = originRotation * rotationY * rotationX;
    }
}
