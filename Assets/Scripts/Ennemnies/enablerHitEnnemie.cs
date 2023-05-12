using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enablerHitEnnemie : MonoBehaviour
{
    public float strundTime = 1f;
    public IAStateMachineManager ISM;
    public EnnemieSMHit ESH;
    private Coroutine co;
    private float counter;
    public void Hit()
    {
        if (co != null)
        {
            StopCoroutine(co);
        }

         co = StartCoroutine(stund());
    }

    IEnumerator stund()
    {
        ISM.enableState(AvailaibleState.hit);
        ISM.disableState(AvailaibleState.focus);
        ISM.disableState(AvailaibleState.fight);
        ESH.resetTimer();
        yield return new WaitForSeconds(strundTime);
        
        ISM.disableState(AvailaibleState.hit);
    }
    
}
