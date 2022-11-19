using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ai.nanosemantics
{
    [RequireComponent(typeof(SessionInitializer))]
    public class TTS : MonoBehaviour, ANNSpeech.ITTSCallback
    {
        [SerializeField]
        AnnSpeechSessionPool session;

        public AudioSource audioSource;
        public int maxClipsCount = 4;
        public bool isPauseSpeak = false;

        public Action OnEndSpeak = delegate { };
        public Action OnPlayAudio = delegate { };
        public Action<AudioClip> OnReadyAudio = delegate { };

        Queue<AudioClip> audioClips = new Queue<AudioClip>();

        private void Start()
        {
            StartCoroutine(AudioPlaying());
        }

        public void SendText(string text) {
            session.SendTTS(text, this, this);
        }

        public void StopSpeaking() {
            audioClips.Clear();
            audioSource.Stop();
            StopAllCoroutines();
            StartCoroutine(AudioPlaying());
        }

        IEnumerator AudioPlaying()
        {
            while (true)
            {
                if (audioClips.Count > 0 && !isPauseSpeak)
                {
                    if(!audioSource.isPlaying)
                        OnPlayAudio?.Invoke();

                    audioSource.clip = audioClips.Dequeue();
                    audioSource.Play();
                    
                    if (audioClips.Count == 0)
                        OnEndSpeak?.Invoke();

                    yield return new WaitForSeconds(audioSource.clip.length);
                }
                else
                    yield return new WaitForSeconds(0.0001f);
            }

        }

        void ANNSpeech.ITTSCallback.OnTTS(float[] audio)
        {
            var clip = AudioClip.Create("", audio.Length, 1, session.tts.config.sample_rate, false);
            clip.SetData(audio, 0);

            OnReadyAudio?.Invoke(clip);

            if(audioClips.Count < maxClipsCount)
                audioClips.Enqueue(clip);
        }
    }
}
