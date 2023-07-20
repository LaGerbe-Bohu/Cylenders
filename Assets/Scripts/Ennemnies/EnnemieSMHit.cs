using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;

public class EnnemieSMHit : MachineState
{
    public float force = 5f;
    public float timer = 0.5f;
    public Transform stateMan;
    public MobInput mobInput;
    public Rigidbody RB;
    private Vector3 direction;
    private float counter;
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
        
        if (RB == null)
        {
            RB = this.transform.parent.GetComponent<Rigidbody>();
        }
        
        player = GameManager.instance.player;
        counter = timer;
    }

    public void resetTimer()
    {
        counter = timer;
        direction = Vector3.ProjectOnPlane((this.transform.position- player.position),Vector3.up).normalized;
    }
    public override void updateMachineState()
    {
        mobInput.setDirection(Vector2.zero);
    }
}
