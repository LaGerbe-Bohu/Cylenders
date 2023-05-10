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

    private Vector2 targetPoint;
    private Vector2 originPoint;
    private bool reach = true;
    private Coroutine coroutine;
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



       originPoint = new Vector2(stateMan.position.x,stateMan.position.z);
    }

    public override void updateMachineState()
    {
       if (reach && mobInput )
       {
           if (coroutine != null)
           {
               StopCoroutine(coroutine);
           }
           Debug.Log(coroutine);
           coroutine = StartCoroutine(walkaround());
       }

    }

    IEnumerator walkaround()
    {
       targetPoint = Random.insideUnitCircle;
       reach = false;
       float counter = this.counter;
       
       while (counter > 0)
       {
           
            mobInput.setDirection(targetPoint);

           counter -= Time.deltaTime;
           yield return null;
       }
        
       reach = true;
    }

   
}
