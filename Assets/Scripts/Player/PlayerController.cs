using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    
    public Quest quest;
    
    private NavMeshAgent agent;
    private AnimatorController animator;
    public float speed = 2f;
    public float sprintSpeed = 4f;
    
    private float currentSpeed = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<AnimatorController>();

        agent.updateRotation = true;
        currentSpeed = speed;
    }
    
    void Update()
    {
        if (!agent.hasPath)
        {
            animator.Idle();
            currentSpeed = speed;
        }
        else if(agent.remainingDistance < agent.stoppingDistance)
        {
            StopMove();
            animator.Idle();
            currentSpeed = speed;
            
            quest.quests[quest.currentDialog].Stop();
            StartCoroutine(quest.NextDialog(0));
        }
        agent.speed = currentSpeed;
        if (agent.path.corners.Length != 0)
        {
            foreach (Vector3 pathCorner in agent.path.corners)
            {
                if (agent.path.corners.ToList().IndexOf(pathCorner) == agent.path.corners.Length - 1) return;
                Debug.DrawLine(pathCorner, agent.path.corners[agent.path.corners.ToList().IndexOf(pathCorner) + 1],
                    Color.red);
            }
        }
    }

    public void MoveTo()
    {
        Debug.Log("Move to " + Quest.targetTag);
        GameObject target = GameObject.Find(Quest.targetTag);

        if (target != null)
        {
            Vector3 targetPosition = target.GetComponent<Collider>().transform.position;
            agent.SetDestination(targetPosition);
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
            
            quest.quests[quest.currentDialog].Stop();
            StartCoroutine(quest.NextDialog(3));
            
        }
    }

    public void StopMove()
    {
        agent.ResetPath();
    }
}

public enum Animations
{
    Run,
    Crouch,
    Walk
}
