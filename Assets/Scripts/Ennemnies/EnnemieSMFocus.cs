using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemieSMFocus : MachineState
{
    public Transform stateMan;
    public MobInput mobInput;
    private Transform player;
    // Start is called before the first frame update
    void Start()
    {
        if (stateMan == null)
        {
            stateMan = this.transform.parent;
        }
        
        if (mobInput == null)
        {
            mobInput = this.transform.parent.GetComponent<MobInput>();
        }


        player = GameManager.instance.player;
    }

    public override void updateMachineState()
    {
        stateMan.transform.position = Vector3.MoveTowards(stateMan.transform.position,player.transform.position,Time.deltaTime);

        Vector3 dir = (player.transform.position - stateMan.transform.position).normalized;
        
        mobInput.setDirection(new Vector2(dir.x,dir.z));

        Debug.Log("aggro !");
    }
}
