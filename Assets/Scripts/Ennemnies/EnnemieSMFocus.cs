using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnnemieSMFocus : MachineState
{
    public Transform stateMan;
    public MobInput mobInput;
    
    private Transform player;
    private NavMeshPath path;
    
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
        path = new NavMeshPath();
    }

    
    /*
    public override void updateMachineState()
    {
        
        Vector3 dir = (player.transform.position - stateMan.transform.position).normalized;
        
        mobInput.setDirection(new Vector2(dir.x,dir.z));
    }
    */
    
    
    public override void updateMachineState()
    {
        NavMesh.CalculatePath(this.transform.position, player.transform.position, NavMesh.AllAreas, path);
        
        if (path.corners.Length <= 0) return;
        
        for (int i = 0; i < path.corners.Length - 1; i++)
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);

        Vector3 dir =  (path.corners[1] - stateMan.transform.position);
        
        mobInput.setDirection(new Vector2(dir.x,dir.z));
        
    }
    
    
}
