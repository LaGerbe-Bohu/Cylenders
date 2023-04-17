using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AvailaibleState
{
    walkaround,
    focus,
    hit,
    death,
    run
}

[RequireComponent(typeof(Animator))]
public class IAStateMachineManager : MonoBehaviour
{
    /// <summary>
    /// this script is use to manage de IA state Machine
    /// </summary>
    
    private Animator animator;
    private AvailaibleState AS;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void enableState(AvailaibleState v)
    {
        if (v == AvailaibleState.focus)
        {
            animator.SetBool(v.ToString(),true);
        }
    }

    public void disableState(AvailaibleState v)
    {
        if (v == AvailaibleState.focus)
        {
            animator.SetBool(v.ToString(),false);
        }
    }
    
}