using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class enablerSMFocus : MonoBehaviour
{
    public IAStateMachineManager ISM;
    public float distanceFocus = 1f;
    public float maxDistnaceLoosFocus = 3f;
    private Transform Player;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameManager.instance.player;
    }

    // Update is called once per frame
    void Update()
    {
        
        if ( Vector3.Distance(Player.transform.position,this.transform.position) < distanceFocus )
        {
            ISM.enableState(AvailaibleState.focus);
          
        } 
        
        if (Vector3.Distance(Player.transform.position, this.transform.position) > maxDistnaceLoosFocus)
        {
            ISM.disableState(AvailaibleState.focus);
            
        }
    }
}
