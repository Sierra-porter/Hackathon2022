using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MicController : MonoBehaviour
{
    public ASRController asrController;
    public bool blockKey = false;

    public KeyCode keyToPress = KeyCode.Space;
    public bool isMute = true;
    public Image active;
    public Image mute;
    

    void Update()
    {
        if(!blockKey){
            if (Input.GetKeyDown(keyToPress))
            {
                if (!asrController.aSR.isASRProcessing)
                {
                    asrController.startRecord();
                }
                isMute = false;
            }
            else if(Input.GetKeyUp(keyToPress))
            {
                if (asrController.aSR.isASRProcessing)
                {
                    asrController.stopRecord();
                }
                isMute = true;
            }
        }
        else
        {
            isMute = true;
        }
        
        if (isMute)
        {
            mute.enabled = true;
            active.enabled = false;
        }
        else
        {
            mute.enabled = false;
            active.enabled = true;
        }
    }
}
