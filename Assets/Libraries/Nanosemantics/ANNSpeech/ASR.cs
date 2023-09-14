using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ai.nanosemantics;
using UnityEngine.UI;

namespace ai.nanosemantics
{
    [RequireComponent(typeof(SessionInitializer))]
    public class ASR : MonoBehaviour, ANNSpeech.IASRCallback
    {
        [SerializeField]
        AnnSpeechSessionPool session;

        public int microphoneIndex = 0;
        public ASRSendingMode aSRSendingMode = ASRSendingMode.Partial;
        public bool endRecoringWhenAnswer = false;
        public bool isRecordAllowed = true;
        public bool isASRProcessing = false;
        public string recognizedText;

        public Action<string> OnAsrMessage = delegate { };

        AudioClip microphoneClip;
        ANNSpeech.SessionASR asr;
        int position1, position2;

        public enum ASRSendingMode
        {
            Partial = 0,
            Single = 1
        }

        void Start()
        {
            StartCoroutine(ASRAnswerListner());
        }

        public void StartRecoring()
        {
            if (!isRecordAllowed)
                return;

            if (asr != null)
                asr.Dispose();

            microphoneClip = Microphone.Start(Microphone.devices[microphoneIndex], true, 3500, 16000);
            position1 = Microphone.GetPosition(Microphone.devices[microphoneIndex]);
            recognizedText = "";
            isASRProcessing = true;

            if (aSRSendingMode == ASRSendingMode.Partial)
            {
                asr = session.pool.NewSessionASR(ref session.asr.config);
                StartCoroutine(FrameSending());
            }
        }

        public void StopRecoring()
        {
            position2 = Microphone.GetPosition(Microphone.devices[microphoneIndex]);

            if (position2 <= position1)
                return;

            var frames = new float[position2 - position1];
            microphoneClip.GetData(frames, position1);

            if (aSRSendingMode == ASRSendingMode.Single)
            {
                if (asr != null)
                    asr.Dispose();

                asr = session.pool.NewSessionASR(ref session.asr.config);
            }

            if (asr != null)
            {
                asr.SendFrames(frames);
                asr.SendControlMsg(ANNSpeech.ControlMsg.EOP);
                Microphone.End(Microphone.devices[microphoneIndex]);
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

                    if ((endRecoringWhenAnswer || !isASRProcessing) && asr != null)
                    {
                        isASRProcessing = false;
                        Microphone.End(Microphone.devices[microphoneIndex]);
                        asr.Dispose();
                        asr = null;
                    }
                } else if (!isASRProcessing && asr != null)
                {
                    Microphone.End(Microphone.devices[microphoneIndex]);
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
                    position2 = Microphone.GetPosition(Microphone.devices[microphoneIndex]);

                    if (position2 > position1)
                    {
                        float[] frames = new float[position2 - position1];

                        microphoneClip.GetData(frames, position1);

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
