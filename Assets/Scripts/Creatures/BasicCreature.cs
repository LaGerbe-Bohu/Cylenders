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

    private bool stadalone = false;
    
    public void Initialization(GeneticCube gen)
    {
        nn = new NN(6, 100, 1);
        prewarm(gen);
        
  
    }

    public void Initialization(saveNN saveNN,Vector3 position,Vector3 origin,float time)
    {
        stadalone = true;
        nn = new NN(6, 100, 1);

        nn.wi = saveNN.wi;
        nn.wo = saveNN.wo;
        

        
        StartCoroutine(CalculDistance(position,origin,time));
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
    


    IEnumerator CalculDistance(Vector3 position,Vector3 origin,float time)
    {
        this.transform.position = origin;
        this.transform.rotation = Quaternion.LookRotation(Vector3.forward);
        float idx = 0;
        while (idx < 5.0f/10.0f)
        {
            f.RB.constraints = RigidbodyConstraints.FreezeAll;
            idx+=Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        f.RB.constraints = RigidbodyConstraints.None;
        this.transform.rotation = Quaternion.LookRotation(Vector3.forward);
        
        while (idx < time)
        {
            Inputs i = new Inputs();
            i.intputs = new List<float>();
         
            Vector3 dir = (position - this.transform.position).normalized;
            
            i.intputs.Add(dir.x);
            i.intputs.Add(dir.y);
            i.intputs.Add(dir.z);
            
            i.intputs.Add(this.transform.up.x);
            i.intputs.Add(this.transform.up.y);
            i.intputs.Add(this.transform.up.z);
            
            List<float> d2 = nn.Update(i.intputs);

            f.MoveNageoire(Mathf.Abs( d2[0]));

            idx+=Time.fixedDeltaTime;


            yield return new WaitForFixedUpdate();
        }

        
      //  while (f.RB.velocity.magnitude > 0.0f)
        //{
        //    yield return new WaitForFixedUpdate();
        //}

        bestDistance  += (float)System.Math.Round( Vector3.Distance(this.transform.position, position),2);
        f.RB.constraints = RigidbodyConstraints.FreezeAll;
    }
    


    public IEnumerator fitness(GeneticCube gen)
    {

        var c = CalculDistance(gen.target.position, gen.gameObject.transform.position, gen.TimeSumulation);
        yield return StartCoroutine(c);

        //while ( fitnessCalculation.Count > 0)
        //{
        //    var c = fitnessCalculation.Dequeue();
        //    yield return StartCoroutine(c);
        //}
       
        gen.nbCorotine--;
    }

    
    
}
