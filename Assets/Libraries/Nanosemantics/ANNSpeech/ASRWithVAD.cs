using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ai.nanosemantics
{
    [RequireComponent(typeof(SessionInitializer), typeof(VAD))]
    public class ASRWithVAD : MonoBehaviour, ANNSpeech.IASRCallback
    {
        [SerializeField]
        AnnSpeechSessionPool session;

        public VAD vad;
        public ASRSendingMode aSRSendingMode = ASRSendingMode.Partial;
        public bool isRecordAllowed = true;
        public bool isASRProcessing = false;
        public string recognizedText;

        public Action<string> OnAsrMessage = delegate { };

        ANNSpeech.SessionASR asr;
        int position1, position2;

        public enum ASRSendingMode
        {
            Partial = 0,
            Single = 1
        }

        void Start()
        {
            vad.OnEndVoiceDetect += OnSignalUndetected;
            vad.OnVoiceDetect += OnSignalDetected;
            StartCoroutine(ASRAnswerListner());
        }

        void OnSignalDetected()
        {
            if (!isRecordAllowed)
                return;

            position1 = Microphone.GetPosition(Microphone.devices[vad.microphoneIndex]) - (int)((16000) * 1f);
            recognizedText = "";
            isASRProcessing = true;

            if (aSRSendingMode == ASRSendingMode.Partial)
            {
                asr = session.pool.NewSessionASR(ref session.asr.config);
                StartCoroutine(FrameSending());
            }
        }

        void OnSignalUndetected()
        {
            position2 = Microphone.GetPosition(Microphone.devices[vad.microphoneIndex]);

            if (position2 <= position1)
                return;

            var frames = new float[position2 - position1];
            vad.micClip.GetData(frames, position1);

            if (aSRSendingMode == ASRSendingMode.Single)
                asr = session.pool.NewSessionASR(ref session.asr.config);

            if (asr != null)
            {
                asr.SendFrames(frames);
                asr.SendControlMsg(ANNSpeech.ControlMsg.EOP);
            }

            isASRProcessing = false;
        }

        public bool OnASR(ANNSpeech.ASREvent ev, string text, string start_timestamp, string end_timestamp, double confidence, bool is_final)
        {
            if (is_final)
            {
                if (text != "")
                {
                    recognizedText = text;
                    OnAsrMessage?.Invoke(recognizedText);
                    asr.Dispose();
                    asr = null;
                }
                return true;
            }
            else
                return false;
        }

        IEnumerator ASRAnswerListner()
        {
            while (true)
            {
                if (asr != null)
                {
                    var code = asr.Recv(this);

                    if (ANNSpeech.Code.FINISH == code || ANNSpeech.IsError(code))
                    {
                        asr.Dispose();
                        asr = session.pool.NewSessionASR(ref session.asr.config);
                    }
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        IEnumerator FrameSending()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);

                if (aSRSendingMode != ASRSendingMode.Partial)
                    yield break;

                if (isASRProcessing && asr != null)
                {
                    position2 = Microphone.GetPosition(Microphone.devices[vad.microphoneIndex]);

                    if (position2 > position1)
                    {
                        float[] frames = new float[position2 - position1];

                        vad.micClip.GetData(frames, position1);

                        asr.SendFrames(frames);

                        position1 = position2;
                    }
                }
                else
                    yield break;
            }
        }

    }
}