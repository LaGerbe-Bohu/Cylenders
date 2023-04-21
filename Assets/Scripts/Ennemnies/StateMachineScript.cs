using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[System.Serializable]
public struct StateInfo
{
    public AvailaibleState stateName;
    public MachineState machineState;
}

[RequireComponent(typeof(Animator))]
public class StateMachineScript : MonoBehaviour
{
    public List<StateInfo> stateList;

    private Animator animator;
    private AnimatorStateInfo asi;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
      
    }

    // Update is called once per frame
    void Update()
    {
        asi = animator.GetCurrentAnimatorStateInfo(0);
        for (int i = 0; i < stateList.Count; i++)
        {
         
            if (asi.IsName(stateList[i].stateName.ToString()))
            {
                Debug.Log(stateList[i].stateName);
                stateList[i].machineState.updateMachineState();
            }
                
        }
        
    }
}
