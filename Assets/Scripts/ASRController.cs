using UnityEngine;
using ai.nanosemantics;

public class ASRController : MonoBehaviour
{
    public ASR aSR;
    [HideInInspector] public bool startRecord = false;
    [HideInInspector] public bool stopRecord = false;
    public string lastResult = "";

    void Start()
    {
        startRecord = false;
        stopRecord = false;
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
}
