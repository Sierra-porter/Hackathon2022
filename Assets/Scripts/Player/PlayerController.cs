using System;
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

    void test()
    {
        Debug.Log("test");
    }
    
    public void MoveTo(Objects obj, Animations anim)
    {
        Debug.Log("Move to " + obj);
        if(obj == Objects.None) return;
        if (!_nearObjects.Any(x => x.CompareTag($"{obj}")))
        {
            Debug.Log($"No {obj} found");
            return;
        }

        switch (anim)
        {
            case Animations.Walk:
                animator.Walk(true);
                _currentSpeed = speed;
                break;
            case Animations.Run:
                animator.Run(true);
                _currentSpeed = sprintSpeed;
                break;
            case Animations.Crouch:
                animator.Crouch(true);
                _currentSpeed = speed / 1.5f;
                break;
        }
        switch(obj)
        {
            case Objects.Wall:
                agent.SetDestination(_nearObjects.Find(x => x.CompareTag("Wall")).transform.position);
                break;
            case Objects.Barrel:
                agent.SetDestination(_nearObjects.Find(x => x.CompareTag("Barrel")).transform.position);
                break;
        }
        
        /*agent.transform.rotation = Quaternion.LookRotation(position - transform.position);*/
    }

    public void StopMove()
    {
        agent.ResetPath();
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

public enum Objects
{
    None,
    Wall,
    Barrel,
    Container,
    Generator,
    Rops,
    TrashCan,
    WoodBox
}

public enum Animations
{
    Run,
    Crouch,
    Walk
}
