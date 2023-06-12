using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CreatureMovement : MonoBehaviour
{
    public Rigidbody creature;
    public float speed = 0.001f;
    public float torqueSpeed = 0.001f;
    public Rigidbody RB;
    public HingeJoint Hj;
    private Vector3 baycentre;
    public List<HingeJoint> arms;
    // Start is called before the first frame update
    void Start()
    {
        creature = this.transform.parent.GetComponent<CreaturesGenerator>().RBofCreature;
        RB = this.GetComponent<Rigidbody>();
        Hj = GetComponent<HingeJoint>();
        arms = new List<HingeJoint>();
    }


    private void OnDrawGizmos()
    {
        if (arms.Count <= 0) return;
        Vector3 p= new Vector3();
        for (int i = 0; i < arms.Count; i++)
        {
            p += (arms[i].transform.position - this.transform.position).normalized;
        }

        Gizmos.color = Color.yellow;
        p = this.transform.position + (p).normalized * 2f;
        Gizmos.DrawSphere(p,.1f);
        
        for (int i = 0; i < arms.Count; i++)
        {
            Gizmos.color = Color.yellow;
           Gizmos.DrawLine(p,arms[i].transform.position);
           Vector3 normal = Vector3.Cross((p-arms[i].transform.position).normalized, (p - this.transform.position).normalized);
           Gizmos.color = Color.green;
           Gizmos.DrawRay(p,normal*3f);
        }


        Gizmos.DrawLine(p,this.transform.position);

      


    }

    // Update is called once per frame
    void Update()
    {
        if (RB.angularVelocity.magnitude > 5f)
        {
            creature.AddForce(creature.transform.up*speed,ForceMode.VelocityChange);
        }

        if (Input.GetKeyDown(KeyCode.E) && arms.Count > 0)
        {
         
            for (int i = 0; i < arms.Count; i++)
            {
                
            }
            
            
        }
        else
        {
          
        }
        
    }
}
