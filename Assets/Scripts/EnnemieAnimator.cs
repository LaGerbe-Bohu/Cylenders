using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnnemieAnimator : MonoBehaviour
{
    public Animator animator;

    private void Start()
    {
        animator.SetFloat("Random",1f + Random.Range(-0.01f,0.01f));
    }

    public void SetWalk(bool v)
    {
        animator.SetBool("walk",v);
    }
    
    public void SetIdle()
    {
        animator.SetBool("walk",false);
        animator.SetBool("attack",false);
    }
    
    
    public void SetAttack(bool v)
    {
        animator.SetBool("attack",v);
    }
    
}
