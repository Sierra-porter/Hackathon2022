using System;
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
    private int currentDialog = 0;
    
    [SerializeField] public static string targetTag;

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
}

[Serializable]
public class Dialog
{
    [HideInInspector] public static TTSController ttsController;
    [HideInInspector] public static ASRController asrController;

    [SerializeField] public List<String> heroReplics;
    [SerializeField] public List<Answer> playerAnswers;

    [HideInInspector] public bool isStarted = false;
    [HideInInspector] public bool isCompleted = false;
    
    private bool fakeAfterTalking = true;
    
    public string targetTag;


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

        ttsController.tTS.OnEndSpeak = null;
        asrController.startRecord = true;
        asrController.aSR.OnAsrMessage += getMessage;
        
        Debug.Log("StartAfterTalking");
    }

    public void getMessage(string text)
    {
        Debug.Log($"message: {text}");
        if(playerAnswers.Exists(x => x.answersAllies.ConvertAll(c => c.ToLower()).Any(r => r.Contains(text.ToLower()))))
        {
            asrController.stopRecord = true;
            UnityEvent action = playerAnswers.Find(x => x.answersAllies.ConvertAll(o => o.ToLower()).Any(r => r.Contains(text.ToLower()))).action;
            
            action?.Invoke();

            isCompleted = true;
        }
        else
        {
            ttsController.sendMessage("Я не понял вас. Повторите пожалуйста.");
        }
    }
}

[Serializable]
public class Answer
{
    [SerializeField] public List<String> answersAllies;
    [SerializeField] public UnityEvent action;
}