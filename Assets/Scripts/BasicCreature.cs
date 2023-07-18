using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using System.IO;

public class BasicCreature : MonoBehaviour
{
    public NN nn;
    private Queue<IEnumerator> fitnessCalculation;
    public float bestDistance = 0.0f;
    public Transform target;
    
    public void Initialization()
    {
        nn = new NN(3, 5, 3);
        prewarm();
    }

    public void prewarm()
    {
        fitnessCalculation = new Queue<IEnumerator>();
        fitnessCalculation.Enqueue(CalculDistance(this.transform.position + new Vector3(4,0,0)));
        fitnessCalculation.Enqueue(CalculDistance(this.transform.position + new Vector3(-4,0,0)));
        fitnessCalculation.Enqueue(CalculDistance(this.transform.position + new Vector3(0,0,4)));
        fitnessCalculation.Enqueue(CalculDistance(this.transform.position + new Vector3(0,0,-4)));
        bestDistance = 0;
    }
    


    IEnumerator CalculDistance(Vector3 position)
    {

        int idx = 0;
        this.transform.position = new Vector3(0,0,0);

        
        while (idx < 10)
        {
            Inputs i = new Inputs();
            i.intputs = new List<float>();
         
            Vector3 dir = (position - this.transform.position).normalized;
            
            i.intputs.Add(dir.x);
            i.intputs.Add(dir.y);
            i.intputs.Add(dir.z);

            Vector3 old = this.transform.position;
            List<float> d2 = nn.Update(i.intputs);

            Vector3 output = new Vector3(d2[0], d2[1], d2[2]);
            this.transform.position += output.normalized;

            Debug.DrawLine(old,this.transform.position,Color.magenta,3000f);
            idx++;
            yield return new WaitForSeconds(1f);
        }

        bestDistance  += Vector3.Distance(this.transform.position, position);
        
    }
    
    public IEnumerator fitness(GeneticCube gen)
    {
        while ( fitnessCalculation.Count > 0)
        {
            var c = fitnessCalculation.Dequeue();
            yield return StartCoroutine(c);
        }

        gen.nbCorotine--;
    }



}
