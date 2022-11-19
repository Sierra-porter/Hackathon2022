using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Quest : MonoBehaviour
{
    [SerializeField] public List<Dialog> quests = new List<Dialog>();
    private  TTSController ttsController;
    private ASRController asrController;
    private int currentDialog = 0;
    void Start()
    {
        ttsController = GetComponent<TTSController>();
        asrController = GetComponent<ASRController>();
    }

    private void Update()
    {
        if(quests[currentDialog].isCompleted)
        {
            if (currentDialog >= quests.Count) return;
            currentDialog++;
        }
        else
        {
            quests[currentDialog].Start(ttsController,asrController);
            if(quests[currentDialog].playerAnswers.Contains(asrController.lastResult))
            {
                quests[currentDialog].isCompleted = true;
                
            }
            return;
        }
    }
}

[Serializable]
public class Dialog
{
    [SerializeField] public List<String> heroReplics;
    [SerializeField] public List<String> playerAnswers;
    
    [HideInInspector] public bool isStarted = false;
    [HideInInspector] public bool isCompleted = false;
    
    
    public void Start(TTSController ttsController, ASRController asrController)
    {
        if(isStarted) return;
        ttsController.sendMessage(heroReplics[new Random().Next(heroReplics.Count)]);
        isStarted = true;

        asrController.startRecord = true;
        //asrController
    }
}