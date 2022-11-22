using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitchScript : MonoBehaviour
{
    public List<Camera> cameras;
    public KeyCode nextKey = KeyCode.RightArrow;
    public KeyCode prevKey = KeyCode.LeftArrow;
    public static Camera currentCamera;
    
    
    void Start()
    {
        if(cameras.Count == 0) return;
        foreach(Camera cam in cameras)
        {
            cam.enabled = false;
        }
        currentCamera = cameras[0];
        currentCamera.enabled = true;
    }
    
    void Update()
    {
        if(currentCamera == null) return;
        if(Input.GetKeyDown(nextKey))
        {
            currentCamera.enabled = false;
            
            currentCamera = cameras.IndexOf(currentCamera) + 1 < cameras.Count ? cameras[cameras.IndexOf(currentCamera) + 1] : cameras[0];
            currentCamera.enabled = true;
        }
        if(Input.GetKeyDown(prevKey))
        {
            currentCamera.enabled = false;
            
            currentCamera = cameras.IndexOf(currentCamera) - 1 >= 0 ? cameras[cameras.IndexOf(currentCamera) - 1] : cameras[cameras.Count - 1];
            currentCamera.enabled = true;
        }
    }
}
