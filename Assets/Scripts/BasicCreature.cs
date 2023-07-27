using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using System.IO;
using Random = UnityEngine.Random;


public class BasicCreature : MonoBehaviour
{
    public NN nn;
    private Queue<IEnumerator> fitnessCalculation;
    public float bestDistance = 0.0f;
    public Transform target;
    public Fish f;
    public void Initialization(GeneticCube gen)
    {
        nn = new NN(6, 50, 1);
        prewarm(gen);
        
        
        
    }

    public void prewarm(GeneticCube gen)
    {
        fitnessCalculation = new Queue<IEnumerator>();
        fitnessCalculation.Enqueue(CalculDistance( gen.transform.position + new Vector3(4,0,0),gen.transform.position,gen.TimeSumulation));
        fitnessCalculation.Enqueue(CalculDistance(gen.transform.position + new Vector3(-4,0,0),gen.transform.position,gen.TimeSumulation));
        fitnessCalculation.Enqueue(CalculDistance(gen.transform.position + new Vector3(0,0,4),gen.transform.position,gen.TimeSumulation));
        fitnessCalculation.Enqueue(CalculDistance(gen.transform.position + new Vector3(0,0,-4),gen.transform.position,gen.TimeSumulation));
        fitnessCalculation.Enqueue(CalculDistance(gen.transform.position + new Vector3(0,4,0),gen.transform.position,gen.TimeSumulation));
        fitnessCalculation.Enqueue(CalculDistance(gen.transform.position + new Vector3(0,-4,0),gen.transform.position,gen.TimeSumulation));
        bestDistance = 0;
  
    }
    


    IEnumerator CalculDistance(Vector3 position,Vector3 origin,int time)
    {
        this.transform.position = origin;
        float idx = 0;
        while (idx <5)
        {
            idx+=Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        this.transform.rotation = Quaternion.LookRotation(Vector3.forward);
    

        Vector3 oldPosition = this.transform.position;
        while (idx < time)
        {
            Inputs i = new Inputs();
            i.intputs = new List<float>();
         
            Vector3 dir = (position - this.transform.position).normalized;
            
            i.intputs.Add(dir.x);
            i.intputs.Add(dir.y);
            i.intputs.Add(dir.z);
            
            i.intputs.Add(this.transform.up.x);
            i.intputs.Add(this.transform.up.z);
            i.intputs.Add(this.transform.up.y);
            
            List<float> d2 = nn.Update(i.intputs);

            f.MoveNageoire(Mathf.Abs( d2[0]));

            idx+=Time.fixedDeltaTime;


            yield return new WaitForFixedUpdate();
        }

        while (f.RB.velocity.magnitude > 0.0f)
        {
            yield return new WaitForFixedUpdate();
        }
        
        bestDistance  += Vector3.Distance(this.transform.position, position);
    }
    
    
    public IEnumerator fitness(GeneticCube gen)
    {
    
        var c = CalculDistance( gen.target.position,gen.gameObject.transform.position,gen.TimeSumulation);
       yield return StartCoroutine(c);
    
       //while ( fitnessCalculation.Count > 0)
       // {
        //    var c = fitnessCalculation.Dequeue();
       //     yield return StartCoroutine(c);
     //   }
       
        gen.nbCorotine--;
    }

    
    
}
