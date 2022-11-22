using UnityEngine;
using ai.nanosemantics;

public class ASRController : MonoBehaviour
{
    public ASR aSR;
    public string lastResult = "";

    public void startRecord()
    {
        aSR.StartRecoring();
    }
    
    public void stopRecord()
    {
        aSR.StopRecoring();
    }
}
