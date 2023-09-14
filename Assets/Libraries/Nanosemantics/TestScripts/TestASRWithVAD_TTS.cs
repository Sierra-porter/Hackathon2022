using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ai.nanosemantics;

public class TestASRWithVAD_TTS : MonoBehaviour
{
    public ASRWithVAD aSRWithVAD;
    public TTS tTS;
    // Start is called before the first frame update
    void Start()
    {
        aSRWithVAD.OnAsrMessage += ASRMessage;
        aSRWithVAD.vad.OnVoiceDetect += VADDetect;
        aSRWithVAD.vad.OnEndVoiceDetect += VADUndetect;
        tTS.OnReadyAudio += TTSMessage;
    }

    void VADDetect()
    {

        Debug.Log("VAD message: Voice detected");
    }

    void VADUndetect()
    {

        Debug.Log("VAD message: Voice undetected");
    }

    void ASRMessage(string message)
    {
        Debug.Log("ASR message: " + message);
        Debug.Log("TTS: Send text");
        tTS.SendText(message);
    }

    void TTSMessage(AudioClip clip)
    {
        Debug.Log("TTS: Ready audio. Length is: " + clip.length);
    }
}
