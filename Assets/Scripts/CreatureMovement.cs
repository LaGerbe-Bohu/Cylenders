using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;


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
    }


    private void OnDrawGizmos()
    {
        /*
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

      */


    }

    public void Move(Vector3 localDir)
    {
        for (int i = 0; i < arms.Count; i++)
        {
              

            if (arms.Count <= 1)
            {
                arms[i].axis = localDir;
                Vector3 dir = this.transform.TransformDirection(localDir);
           
                this.RB.AddForce(dir.normalized*100f,ForceMode.VelocityChange);   
            }
              
        }
            
        
        /*
        if (arms.Count > 1)
        {
                
            Vector3 p= new Vector3();
            for (int i = 0; i < arms.Count; i++)
            {
                p += (arms[i].transform.position - this.transform.position).normalized;
            }
                
            p = this.transform.position + (p).normalized * 2f;
                
            this.RB.AddForce( (this.transform.position - p).normalized*20F,ForceMode.VelocityChange);                
        }
        else
        {
            Vector3 dir = this.transform.TransformDirection(localDir); 
            //this.RB.AddForce( (this.transform.position - (this.transform.position+  dir)).normalized*20F,ForceMode.VelocityChange);   
            
        }

*/

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
            Vector3 dir = new Vector3(Random.Range(-1, 1),Random.Range(-1, 1),Random.Range(-1, 1));
            Move(dir); 
            
        }


  
        
    }
}
