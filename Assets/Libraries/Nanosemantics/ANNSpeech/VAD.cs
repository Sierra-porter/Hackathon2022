using System;
using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebRtcVadSharp;

namespace ai.nanosemantics
{
    public class VAD : MonoBehaviour
    {
        public int microphoneIndex = 0;
        public MicrophoneSignalMultiplier microphoneSignalMultiplier = MicrophoneSignalMultiplier.X4;
        public OperatingMode vADOperatingMode;

        [HideInInspector]
        public AudioClip micClip;

        public Action OnVoiceDetect = delegate { };
        public Action OnEndVoiceDetect = delegate { };

        bool voiceDetected = false;
        int nonVoiceDetectCounter = 0;
        WebRtcVad vad = new WebRtcVad();
        int currentMicrophone = 0;

        public enum MicrophoneSignalMultiplier
        {
            X1 = 1,
            X2 = 2,
            X3 = 3,
            X4 = 4,
            X5 = 5,
            X6 = 6,
            X7 = 7,
            X8 = 8,
            X9 = 9,
            X10 = 10
        }

        void Awake()
        {
            currentMicrophone = microphoneIndex;
            micClip = Microphone.Start(Microphone.devices[microphoneIndex], true, 3500, 16000);

            StartCoroutine(DetectionUpdate());
        }

        IEnumerator DetectionUpdate()
        {
            int position = 0;
            bool checkResult;

            while (true)
            {
                if (currentMicrophone != microphoneIndex)
                {
                    Microphone.End(Microphone.devices[currentMicrophone]);
                    micClip = Microphone.Start(Microphone.devices[microphoneIndex], true, 3500, 16000);
                    position = 0;
                    currentMicrophone = microphoneIndex;
                }

                checkResult = CheckFrames(ref position);

                if (checkResult && !voiceDetected)
                {
                    voiceDetected = true;
                    OnVoiceDetect?.Invoke();
                    StartCoroutine(NonVoiceDetection());
                }
                else if (checkResult)
                {
                    nonVoiceDetectCounter = 0;
                }

                yield return new WaitForSeconds(0.2f);
            }

        }

        bool CheckFrames(ref int position)
        {
            int currentPosition = Microphone.GetPosition(Microphone.devices[microphoneIndex]);

            if (currentPosition > position)
            {
                float[] frames = new float[currentPosition - position];
                micClip.GetData(frames, position);

                position = currentPosition;

                short[] convertedFrames = new short[frames.Length];

                for (int i = 0; i < convertedFrames.Length; i++)
                    convertedFrames[i] = Int24bitTo16bit(FloatTo24bit(frames[i] * (float)microphoneSignalMultiplier));

                var byteArray = new byte[frames.Length * 2];

                Buffer.BlockCopy(convertedFrames, 0, byteArray, 0, byteArray.Length);

                return DoesFrameContainSpeech(byteArray);
            }

            return false;
        }

        bool DoesFrameContainSpeech(byte[] audioFrame)
        {
            vad.OperatingMode = vADOperatingMode;
            return vad.HasSpeech(audioFrame, SampleRate.Is16kHz, FrameLength.Is20ms);
        }

        short Int24bitTo16bit(int val)
        {
            return (short)(val << 8);
        }

        int FloatTo24bit(float val)
        {
            return (int)Mathf.Round(Mathf.Clamp(val, -1, 1) * 0x7FFFFF) & 0xFFFFFF;
        }

        IEnumerator NonVoiceDetection()
        {
            nonVoiceDetectCounter = 0;

            while (true)
            {
                yield return new WaitForSeconds(0.1f);
                nonVoiceDetectCounter++;

                if (nonVoiceDetectCounter > 10)
                {
                    voiceDetected = false;
                    OnEndVoiceDetect?.Invoke();
                    yield break;
                }
            }
        }

    }
}
