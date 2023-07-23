using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnnemieSMWalkAround : MachineState
{
    public Transform stateMan;
    public MobInput mobInput;
    public float counter = 2f;

    public EnnemieAnimator EA;
    
    private Vector2 targetPoint;
    private Vector2 originPoint;
    private Coroutine coroutine;
    private float cc;
    private void Start()
    {
       if (stateMan == null)
       {
           stateMan = this.transform.parent;
       }
       
       if (mobInput == null)
       {
           mobInput = this.transform.parent.GetComponent<MobInput>();
       }

       cc = this.counter;
       targetPoint = Random.insideUnitCircle;
       
       originPoint = new Vector2(stateMan.position.x,stateMan.position.z);
    }

    public override void updateMachineState()
    {
       if (mobInput )
       {
           mobInput.setDirection(targetPoint);
           cc -= Time.deltaTime;
           if(EA != null) 
               EA.SetWalk(true);
           
           if (cc < 0)
           {
             
               cc = this.counter;
               targetPoint = Random.insideUnitCircle;
           }
       }
    }
    
}
