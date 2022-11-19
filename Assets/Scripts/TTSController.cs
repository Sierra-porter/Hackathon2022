using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ai.nanosemantics;

public class TTSController : MonoBehaviour
{
    public TTS tTS;
    [TextArea] public string text;
    public bool readySend = false;


    void Start()
    {
        tTS.OnReadyAudio += TTSMessage;
        readySend = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (readySend)
        {
            readySend = false;
            tTS.SendText(text);
        }
    }

    void TTSMessage(AudioClip clip)
    {
        Debug.Log("TTS: Ready audio. Length is: " + clip.length);
    }
}
