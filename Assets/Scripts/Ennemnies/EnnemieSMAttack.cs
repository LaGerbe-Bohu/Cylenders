using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemieSMAttack : MachineState
{
    public Transform stateMan;
    public MobInput mobInput;
    public float Dps = 1;
    public Rigidbody rb;
    public EnnemieInformation EI;
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
        counter = 0.0f;
    }

    
    public override void updateMachineState()
    {
        mobInput.setDirection(Vector2.zero);

        if (counter < 0)
        {
            counter = Dps;
            
            GM.PlayerHurt(EI.dommage);
        }

    }

    private void Update()
    {
        if (counter >= 0)
        {
            counter -= Time.deltaTime;    
        }
        
    }
}
