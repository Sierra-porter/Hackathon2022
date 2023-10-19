using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ai.nanosemantics
{
    public class SessionInitializer : MonoBehaviour
    {
        public AnnSpeechSessionPool pool;

        void Awake()
        {
            pool.Init();
        }

    }
}