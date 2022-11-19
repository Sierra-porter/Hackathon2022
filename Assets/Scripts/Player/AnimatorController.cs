using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Walk(bool status)
    {
        animator.SetBool("Walk", status);
    }

    public void Crouch(bool status)
    {
        animator.SetBool("Crouch", status);
    }

    public void Run(bool status)
    {
        animator.SetBool("Run", status);
    }

    public void Idle()
    {
        Crouch(false);
        Run(false);
        Walk(false);
    }
}
