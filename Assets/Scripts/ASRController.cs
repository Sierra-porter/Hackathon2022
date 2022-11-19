using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ai.nanosemantics;

public class ASRController : MonoBehaviour
{
    public ASR aSR;
    public bool startRecord = false;
    public bool stopRecord = false;
    private PlayerController playerController;
    
    void Start()
    {
        aSR.OnAsrMessage += ASRMessage;
        startRecord = false;
        stopRecord = false;
        
        playerController = GetComponent<PlayerController>();
    }
    
    void Update()
    {
        if (startRecord)
        {
            startRecord = false;
            aSR.StartRecoring();
        }

        if (stopRecord)
        {
            stopRecord = false;
            aSR.StopRecoring();
        }
    }

    void ASRMessage(string message)
    {
        playerController.MoveTo(getObjectFromMessage(message.ToLower()), getActionFromMessage(message.ToLower()));
        Debug.Log("ASR message: " + message);
    }
    
    Objects getObjectFromMessage(string message)
    {
        if (message.Contains("бочк"))
        {
            return Objects.Barrel;
        }
        else if (message.Contains("стен"))
        {
            return Objects.Wall;
        }
        else if (message.Contains("ящи"))
        {
            return Objects.WoodBox;
        }
        else if(message.Contains("верев") || message.Contains("верёв"))
        {
            return Objects.Rops;
        }
        else if (message.Contains("мусор") || message.Contains("бак") || message.Contains("бачо"))
        {
            return Objects.TrashCan;
        }
        else if (message.Contains("конте"))
        {
            return Objects.Container;
        }
        else if (message.Contains("генер"))
        {
            return Objects.Generator;
        }
        else
        {
            return Objects.None;
        }
    }

    Animations getActionFromMessage(string message)
    {
        if (message.Contains("бег"))
        {
            return Animations.Run;
        }
        else if (message.Contains("ид"))
        {
            return Animations.Walk;
        }
        else if (message.Contains("крад"))
        {
            return Animations.Crouch;
        }
        else
        {
            return Animations.Crouch;
        }
    }
}
