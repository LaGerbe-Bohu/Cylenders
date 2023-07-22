using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enablerSMFight : MonoBehaviour
{
    public IAStateMachineManager ISM;
    public float distanceFight = .5f;
    private Transform Player;
    public EnnemieAnimator EA;
    
    // Start is called before the first frame update
    void Start()
    {
        Player = GameManager.instance.player;
    }

    void Update()
    {
        if ( Vector3.Distance(Player.transform.position,this.transform.position) < distanceFight )
        {
            ISM.enableState(AvailaibleState.fight);
            ISM.disableState(AvailaibleState.focus);
            EA.SetWalk(false);

        }
        else
        {
            ISM.disableState(AvailaibleState.fight);
        }
        
    }
}
