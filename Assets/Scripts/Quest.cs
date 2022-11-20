using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

public class Quest : MonoBehaviour
{
    [SerializeField] public List<Dialog> quests = new List<Dialog>();
    public TTSController ttsController;
    public ASRController asrController;
    public int currentDialog = 0;
    
    public static string targetTag;
    public static string targetAnimation;

    private void Start()
    {
        ttsController = GetComponent<TTSController>();
        asrController = GetComponent<ASRController>();
        
        Dialog.ttsController = ttsController;
        Dialog.asrController = asrController;
    }

    private void Update()
    {
        if(quests[currentDialog].isCompleted)
        {
            if (currentDialog == quests.Count - 1) return;
            currentDialog++;
        }
        else if(!quests[currentDialog].isStarted)
        {
            quests[currentDialog].Start();
            //StartCoroutine(delay(quests[currentDialog].delayTime));
        }
        else if(quests[currentDialog].isStarted)
        {
            
        }
    }



    public string targetTagProperty
    {
        get => targetTag;
        set => targetTag = value;
    }
    
    public string targetAnimationProperty
    {
        get => targetAnimation;
        set => targetAnimation = value;
    }
}

[Serializable]
public class Dialog
{
    [HideInInspector] public static TTSController ttsController;
    [HideInInspector] public static ASRController asrController;

    [SerializeField] public List<String> heroReplics;
    [SerializeField] public List<Answer> playerAnswers;
    [SerializeField] public int delayTime;
    
    [HideInInspector] public bool isStarted = false;
    [HideInInspector] public bool isCompleted = false;
    
    private bool fakeAfterTalking = true;
    
    [HideInInspector] public string targetTag;

    

    public void Start()
    {
        ttsController.sendMessage(heroReplics[new Random().Next(heroReplics.Count)]);
        isStarted = true;
        ttsController.tTS.OnEndSpeak = StartAfterTalking;
    }

    public void StartAfterTalking()
    {
        if(fakeAfterTalking)
        {
            fakeAfterTalking = false;
            return;
        }

        //ttsController.tTS.OnEndSpeak = null;
        asrController.startRecord = true;
        asrController.aSR.OnAsrMessage += getMessage;
        
        Debug.Log("StartAfterTalking");
    }

    public void getMessage(string text)
    {
        Debug.Log($"message: {text}");
        foreach (Answer playerAnswer in playerAnswers)
        {
            foreach (string playerAnswerAlly in playerAnswer.answersAllies)
            {
                if (text.ToLower().Contains(playerAnswerAlly.ToLower()))
                {
                    asrController.aSR.OnAsrMessage -= getMessage;
                    asrController.stopRecord = true;
                    UnityEvent action = playerAnswer.action;
            
                    action?.Invoke();
                }
                else
                {
                    asrController.aSR.OnAsrMessage -= getMessage;
                    asrController.stopRecord = true;
                    ttsController.sendMessage("Я не понял вас. Повторите пожалуйста.");
                    ttsController.tTS.OnEndSpeak = StartAfterTalking;
                }
            }
        }
        
    }
}

[Serializable]
public class Answer
{
    [SerializeField] public List<String> answersAllies;
    [SerializeField] public UnityEvent action;
}