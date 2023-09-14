using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ai.nanosemantics;

public class TestASRWithVAD : MonoBehaviour
{
    public ASRWithVAD aSRWithVAD;

    void Start()
    {
        aSRWithVAD.OnAsrMessage += ASRMessage;
        aSRWithVAD.vad.OnVoiceDetect += VADDetect;
        aSRWithVAD.vad.OnEndVoiceDetect += VADUndetect;
    }

    void ASRMessage(string message) {

        Debug.Log("ASR message: " + message);
    }

    void VADDetect() {

        Debug.Log("VAD message: Voice detected");
    }

    void VADUndetect() {

        Debug.Log("VAD message: Voice undetected");
    }
}
