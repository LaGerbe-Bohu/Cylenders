using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AvailaibleState
{
    walkaround,
    focus,
    fight,
    death,
    run,
    hit
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
        if (animator == null) return;
        animator.SetBool(v.ToString(),true);
    }

    public bool getStateValue(AvailaibleState v)
    {
        if (animator == null) return false;

        return animator.GetBool(v.ToString());
    }

    public void disableState(AvailaibleState v)
    {
        if (animator == null) return;
        animator.SetBool(v.ToString(),false);
    }
    
}
