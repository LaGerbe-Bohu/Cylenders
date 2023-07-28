using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;


[RequireComponent(typeof(Rigidbody))]
public class Fish : MonoBehaviour
{
    public List<Transform> nageroies;
    public List<Animator> lstAnim;
    public NN nn;
    public Rigidbody RB;
    private Animator anim;
    private void Start()
    {
        Initalize();
      
    }

    // Start is called before the first frame update
    void Initalize()
    {
        RB = GetComponent<Rigidbody>();
        lstAnim = new List<Animator>();
        for (int i = 0; i < nageroies.Count; i++)
        {
            lstAnim.Add(nageroies[i].GetComponent<Animator>());
        }

    }

    public void MoveNageoire(float a)
    {
        if (RB == null)
        {
            Initalize();
        }

        int idx = 0;
        
        float step = 1.0f / (float)nageroies.Count;
        for (float i = 0.0f; i <= 1.0f; i+= step)
        {
            if (i <= a  && a <= i + step)
            {
                break;
            }

            idx++;
        }
        
       lstAnim[idx].SetTrigger("Nage");
        
        Vector3 dir = this.transform.position - nageroies[idx].position;

        RB.AddTorque(nageroies[idx].forward  * (dir.magnitude > 0f ? -1f:1f ) * .01f*7f*Time.fixedDeltaTime*120f ,ForceMode.Impulse);
        RB.AddForce(this.transform.up*.02f*15.0f*Time.fixedDeltaTime*120.0f,ForceMode.Impulse);
        
    }


    public void Rendering()
    {
        this.GetComponent<Renderer>().enabled = !this.GetComponent<Renderer>().enabled;

        for (int i = 0; i < this.nageroies.Count; i++)
        {
            nageroies[i].GetChild(0).GetComponent<Renderer>().enabled = !nageroies[i].GetChild(0).GetComponent<Renderer>().enabled;
        }
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveNageoire(0);
        }
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveNageoire(.35f);
        }
        
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveNageoire(.55f);
        }
        
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveNageoire(1f);
        }
    }
}
