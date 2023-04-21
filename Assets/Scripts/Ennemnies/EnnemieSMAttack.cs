using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemieSMAttack : MachineState
{
    public Transform stateMan;
    public MobInput mobInput;
    public float Dps = 1;
    
    private Transform player;

    private GameManager GM;

    private float counter = 0;
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
        GM = GameManager.instance;
        counter = -1;
    }

    
    public override void updateMachineState()
    {
        mobInput.setDirection(Vector2.zero);

        counter -= Time.deltaTime;

        if (counter < 0)
        {
            counter = Dps;
            
            GM.PlayerHurt(1);
        }

    }
}
