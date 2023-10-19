using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ai.nanosemantics;

public class TestASR_TTS : MonoBehaviour
{
    public ASR aSR;
    public TTS tTS;
    public bool startRecord = false;
    public bool stopRecord = false;
    // Start is called before the first frame update
    void Start()
    {
        aSR.OnAsrMessage += ASRMessage;
        tTS.OnReadyAudio += TTSMessage;
        startRecord = false;
        stopRecord = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (startRecord)
        {
            startRecord = false;
            Debug.Log("ASR: Start Recoring");
            aSR.StartRecoring();
        }

        if (stopRecord)
        {
            stopRecord = false;
            Debug.Log("ASR: Stop Recoring");
            aSR.StopRecoring();
        }
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
