using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    
    private NavMeshAgent agent;
    private AnimatorController animator;
    public float speed = 2f;
    public float sprintSpeed = 4f;
    
    private float _currentSpeed = 0f;
    private List<GameObject> _nearObjects = new List<GameObject>();

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<AnimatorController>();
        DetectObjects(10f);

        agent.updateRotation = true;
        _currentSpeed = speed;
    }
    
    void Update()
    {
        if (!agent.hasPath)
        {
            animator.Idle();
            _currentSpeed = speed;
          
            DetectObjects(15f);
        }
        else if(agent.remainingDistance < 1f)
        {
            StopMove();
            animator.Idle();
            _currentSpeed = speed;
            
            DetectObjects(15f);
        }
        agent.speed = _currentSpeed;
    }

    public void MoveTo()
    {
        Debug.Log("Move to " + Quest.targetTag);
        GameObject target = GameObject.Find(Quest.targetTag);
        
        if (target != null)
        {
            agent.SetDestination(target.GetComponent<Collider>().transform.position);
            switch (Quest.targetAnimation)
            {
                case "Crouch":
                    animator.Crouch(true);
                    break;
                case "Run":
                    animator.Run(true);
                    break;
                case "Walk":
                    animator.Walk(true);
                    break;
            }
            
            Quest quest = GameObject.Find("Quest").GetComponent<Quest>();
            quest.quests[quest.currentDialog].isCompleted = true;
        }
    }

    public void KillShoot()
    {
        Debug.Log("Kill " + Quest.targetTag);
        GameObject target = GameObject.Find(Quest.targetTag);

        if (target != null)
        {
            agent.transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
            target.GetComponent<Animator>().SetBool("Death", true);
                
            Quest quest = GameObject.Find("Quest").GetComponent<Quest>();
            quest.quests[quest.currentDialog].isCompleted = true;
        }
    }

    public void StopMove()
    {
        agent.ResetPath();
    }
    
    IEnumerator KillCountDown(int sec, GameObject target)
    {
        yield return new WaitForSeconds(sec);

        target.GetComponent<Animator>().SetBool("Death", true);
                
        Quest quest = GameObject.Find("Quest").GetComponent<Quest>();
        quest.quests[quest.currentDialog].isCompleted = true;
        /*Ray ray = new Ray(agent.transform.position + new Vector3(0, 0.9f, 0),
            target.transform.position + new Vector3(0, 0.9f, 0));
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 5f);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.gameObject == target)
            {
                target.GetComponent<Animator>().SetBool("Death", true);
                
                Quest quest = GameObject.Find("Quest").GetComponent<Quest>();
                quest.quests[quest.currentDialog].isCompleted = true;
                quest.asrController.stopRecord = true;
            }
            else
            {
                Dialog.ttsController.sendMessage("Я его не вижу!");
            }
        }*/

    }
    
    public void DetectObjects(float radius) {

        Vector3 playerPosition = agent.transform.position;

        var hitColliders = Physics.OverlapSphere(playerPosition, radius);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.layer == 3)
            {
                _nearObjects.Add(hitCollider.gameObject);
            }
        }
        _nearObjects = _nearObjects.OrderBy(x => Vector3.Distance(x.transform.position, playerPosition)).ToList();
    }
}

public enum Animations
{
    Run,
    Crouch,
    Walk
}
