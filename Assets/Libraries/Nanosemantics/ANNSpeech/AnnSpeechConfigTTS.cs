using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ai.nanosemantics
{
    [CreateAssetMenu(fileName = "TTS_Config", menuName = "nanosemantics/config/tts", order = 1)]
    public class AnnSpeechConfigTTS : ScriptableObject
    {
        public ANNSpeech.TTSConfig config;

        [SerializeField]
        TTS_Config _config;

        public void Init() {

            config = new ANNSpeech.TTSConfig(_config.address,_config.token,null,_config.voice,_config.sample_rate,_config.pitch);
            config.end_silence_ms = _config.end_silence_ms;
            config.secure = _config.secure;
        }

    }

    [System.Serializable]
    public class TTS_Config
    {

        public string address;
        public string token;
        public int sample_rate = 8000;
        public string voice = "lj";
        public int channels = 1;
        public float pitch = 1;
        public int end_silence_ms = 0;
        public int secure = 1;
    }

}