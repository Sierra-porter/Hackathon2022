using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = System.Random;

public class Quest : MonoBehaviour
{
    [SerializeField] public List<Dialog> quests = new List<Dialog>();
    public TTSController ttsController;
    public ASRController asrController;
    public MicController micController;
    
    // GUI
    public Image lastImage;
    public List<Image> disableUI;
    
    // Counters
    [HideInInspector] public int currentDialog = 0;
    [HideInInspector] public static int nextDialog = 0;
    
    // target
    public static string targetTag;
    public static string targetAnimation;

    private void Start()
    {
        ttsController = GetComponent<TTSController>();
        asrController = GetComponent<ASRController>();
        micController = GetComponent<MicController>();
        
        Dialog.ttsController = ttsController;
        Dialog.asrController = asrController;
        Dialog.micController = micController;
        
        StartDialog();
    }
    
    
    public IEnumerator NextDialog(int delay)
    {
        if (quests[currentDialog].isLast)
        {
            Debug.Log("end");
            //start end
            lastImage.gameObject.SetActive(true);
            foreach (Image image in disableUI)
            {
                image.gameObject.SetActive(false);
            }
            
            yield break;
        }
        quests[currentDialog].Stop();
        currentDialog = nextDialog;

        yield return new WaitForSeconds(delay);
        StartDialog();
    }

    public void StartDialog()
    {
        quests[currentDialog].Start();
        Debug.Log("next dialog");
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
    public static TTSController ttsController;
    public static ASRController asrController;
    public static MicController micController;

    [SerializeField] public List<String> heroReplics;
    [SerializeField] public List<String> heroWorkReplics;
    [SerializeField] public List<Answer> playerAnswers;
    [SerializeField] public List<String> incorrectAnswersByNPC;
    [SerializeField] public bool isLast;

    private bool fakeAfterTalking = true;



    public void Start()
    {
        ttsController.sendMessage(heroReplics[new Random().Next(heroReplics.Count)]);
        ttsController.tTS.OnEndSpeak = StartAfterTalking;
        micController.blockKey = true;
    }

    public void Stop()
    {
        micController.blockKey = false;
        asrController.aSR.OnAsrMessage = null;
        ttsController.tTS.OnEndSpeak = null;
    }

    public void StartAfterTalking()
    {
        if(fakeAfterTalking)
        {
            fakeAfterTalking = false;
            return;
        }

        ttsController.tTS.OnEndSpeak = null;
        asrController.aSR.OnAsrMessage = getMessage;
        
        micController.blockKey = false;
    }

    public void getMessage(string text)
    {
        Debug.Log($"message: {text}");
        foreach (Answer playerAnswer in playerAnswers)
        {
            foreach (string playerAnswerAlly in playerAnswer.answersAllies)
            {
                foreach (string word in text.Split(" "))
                {
                    int levensteinDistance = Utility.LevenshteinDistance(playerAnswerAlly.ToLower(), word.ToLower());
                    if (levensteinDistance <= 2)
                    {
                        Quest.nextDialog = playerAnswer.nextDialog;
                        
                        micController.blockKey = true;
                        UnityEvent action = playerAnswer.action;
                        ttsController.sendMessage(heroWorkReplics[new Random().Next(heroWorkReplics.Count)]);
                        action?.Invoke();
                        
                        return;
                    }
                }
                
                
            }
        }
        
        
        asrController.aSR.OnAsrMessage -= getMessage;
        asrController.stopRecord();
        
        ttsController.sendMessage(incorrectAnswersByNPC[new Random().Next(incorrectAnswersByNPC.Count)]);
        ttsController.tTS.OnEndSpeak = StartAfterTalking;

    }
}

[Serializable]
public class Answer
{
    [SerializeField] public List<String> answersAllies;
    [SerializeField] public UnityEvent action;
    [SerializeField] public int nextDialog;
}