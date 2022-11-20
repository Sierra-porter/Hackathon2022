using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ai.nanosemantics;

public class TTSController : MonoBehaviour
{
    public TTS tTS;
    [HideInInspector] public string text;
    [HideInInspector] public bool readySend = false;


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

    public void sendMessage(String text) {
		tTS.SendText(text);
	}

    void TTSMessage(AudioClip clip)
    {
        
    }
}
