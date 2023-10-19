using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ai.nanosemantics
{
    [CreateAssetMenu(fileName = "Session", menuName = "nanosemantics/config/session", order = 1)]
    public class AnnSpeechSessionPool : ScriptableObject
    {
        public ANNSpeech.SessionPool pool;
        public AnnSpeechConfigASR asr;
        public AnnSpeechConfigTTS tts;

        public ANNSpeech.Code asr_code;
        public ANNSpeech.Code tts_code;

        AudioClip micClip;

        public void Init() { 
        
            pool = new ANNSpeech.SessionPool(250);

            asr.Init();
            tts.Init();

        }

        public void SendTTS(string text, ANNSpeech.ITTSCallback tTSCallback,MonoBehaviour mono) {

            mono.StartCoroutine(SendToTTS(text,tTSCallback));
        }

        public IEnumerator SendToTTS(string text,ANNSpeech.ITTSCallback tTSCallback) {

            tts.config.text = text;

            using (var tts_session = pool.NewSessionTTS(ref tts.config)){


                while (true) {

                    ANNSpeech.Code code = tts_session.Recv(tTSCallback);

                    tts_code = code;

                    if (ANNSpeech.IsError(code))
                    {
                        Debug.Log("TTS session closed unexpectedly with code " + code + "!");

                        yield break;
                    }
                    else if (code == ANNSpeech.Code.FINISH) {

                   
                        break;
                    }

                    tts_code = ANNSpeech.Code.SUCCESS;
                    yield return new WaitForSeconds(0.1f);

                }
            }

        }

    }


}