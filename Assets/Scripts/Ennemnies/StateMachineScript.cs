using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StateInfo
{
    public AvailaibleState stateName;
    public MachineState machineState;
    public Color debug;
}

[RequireComponent(typeof(Animator))]
public class StateMachineScript : MonoBehaviour
{
    public List<StateInfo> stateList;

    private Animator animator;
    private AnimatorStateInfo asi;

    private Color currentState;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
      
    }


    private void OnDrawGizmos()
    {
        Color c = Color.white;
        asi = animator.GetCurrentAnimatorStateInfo(0);
        for (int i = 0; i < stateList.Count; i++)
        {
            if (asi.IsName(stateList[i].stateName.ToString()))
            {
                c = stateList[i].debug;
            }
        }
        Gizmos.color =c;
        Gizmos.DrawCube(this.transform.position + Vector3.up*2f, Vector3.one * 1f);
   
    }

    // Update is called once per frame
    void Update()
    {
        asi = animator.GetCurrentAnimatorStateInfo(0);
        for (int i = 0; i < stateList.Count; i++)
        {
         
            if (asi.IsName(stateList[i].stateName.ToString()))
            {
                Debug.Log(   stateList[i].stateName + " aller");
                stateList[i].machineState.updateMachineState();
                currentState = stateList[i].debug;
            }
                
        }
        
    }
}
