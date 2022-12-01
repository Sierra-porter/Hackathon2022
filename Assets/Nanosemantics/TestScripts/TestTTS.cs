using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ai.nanosemantics;

public class TestTTS : MonoBehaviour
{
    public TTS tTS;
    [TextArea] public string text;
    public bool readySend = false;

    // Start is called before the first frame update
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
            Debug.Log("TTS: Send text");
            tTS.SendText(text);
        }
    }

    void TTSMessage(AudioClip clip)
    {
        Debug.Log("TTS: Ready audio. Length is: " + clip.length);
    }
}
