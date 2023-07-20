using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class enablerHitEnnemie : MonoBehaviour
{
    public float strundTime = 1f;
    public IAStateMachineManager ISM;
    public EnnemieSMHit ESH;
    private Coroutine co;
    private float counter;
    
    public CharacterController CC;
    public NavMeshAgent NM;
    public void Hit()
    {
        if (co != null)
        {
            StopCoroutine(co);
        }

         co = StartCoroutine(stund());
    }

    private void Update()
    {
        
    }

    IEnumerator stund()
    {
        ISM.enableState(AvailaibleState.hit);
        ISM.disableState(AvailaibleState.focus);
        ISM.disableState(AvailaibleState.fight);
        ESH.resetTimer();
        
        CC.enabled = false;
        NM.enabled = false;

        CC.transform.gameObject.layer = LayerMask.NameToLayer("MobTmp");
        
        yield return new WaitForSeconds(strundTime);
        
        CC.transform.gameObject.layer = LayerMask.NameToLayer("Mob");
        
        CC.enabled = true;
        NM.enabled = true;
        
        ISM.disableState(AvailaibleState.hit);
    }
    
}
