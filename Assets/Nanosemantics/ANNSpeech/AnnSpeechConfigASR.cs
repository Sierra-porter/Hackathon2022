using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ai.nanosemantics
{
    [CreateAssetMenu(fileName = "ASR_Config", menuName = "nanosemantics/config/asr", order = 1)]
    public class AnnSpeechConfigASR : ScriptableObject
    {
        public ANNSpeech.ASRConfig config;

        [SerializeField]
        ASR_Config _confg;

        

        public void Init() {

            config = new ANNSpeech.ASRConfig(_confg.address,_confg.token,_confg.sample_rate,_confg.channels);
            config.language = _confg.language;
            config.secure = _confg.secure;
            config.partial_result_mode = _confg.partial_result_mode;
            config.speech_incomplete_timeout_ms = 300;
            config.speech_complete_timeout_ms = 300;
            config.aggressiveness_mode = _confg.aggressiveness_mode;
        }

    }

    [System.Serializable]
    public class ASR_Config {

        public string address;
        public string token;
        public string language = "en";
        public int sample_rate = 8000;
        public int secure = 1;
        public int channels = 1;
        public int partial_result_mode = 1;
        public int aggressiveness_mode = 1;
    }

}