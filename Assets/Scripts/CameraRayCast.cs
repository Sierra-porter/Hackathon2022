using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRayCast : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    GameObject obj;
    public CameraSwitchScript cameraSwitchScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) &&
                cameraSwitchScript.currentCamera.gameObject.Equals(this.gameObject))
        {
            obj = GetObjectByRaycast(ray, hit);
            if (obj.CompareTag("Cube"))
            {
                Destroy(obj);
            }
        }
    }

    static GameObject GetObjectByRaycast(Ray ray, RaycastHit hit)
    {
        ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Physics.Raycast(ray, out hit);
        return hit.transform.gameObject;
    }
}
