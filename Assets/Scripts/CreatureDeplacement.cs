using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class CreatureDeplacement : MonoBehaviour
{
    public CreaturesGenerator CG;
    public float timer = 1.0f;
    public Transform target;
    private Limb firstLimb;
    public PhysicsScene PS;
    
    [HideInInspector]
    public NN nn;

    [HideInInspector] public float bestDistance;
    private Coroutine fitnessCorotine;
    private Queue<IEnumerator> fitnessCalculation;
    Inputs input = new Inputs();

    private void OnJointBreak(float breakForce)
    {
        foreach (Transform tr in this.transform)
        {
            Destroy(tr.gameObject);
        }
        
        CG.Generator(CG.seed);
        this.firstLimb = CG.firstLimb;
    }

    private void Start()
    {
      
        // Initialize();
    }

    void LoadBest(string name,NN nn)
    {
      
        string fileName = Application.dataPath+"/"+name;
        string jsonString = File.ReadAllText(fileName);
        saveNN sNN = JsonConvert.DeserializeObject<saveNN>(jsonString);

        nn.wi = sNN.wi;
        nn.wo = sNN.wo;
    }
    


    public void prewarm(Genetic gen,int indice)
    {
        firstLimb = CG.firstLimb;
        fitnessCalculation = new Queue<IEnumerator>();

        fitnessCalculation.Enqueue(CalculDistance(this.transform.position + new Vector3(4, 0, 0),gen,indice));
        fitnessCalculation.Enqueue(CalculDistance(this.transform.position + new Vector3(-4, 0, 0),gen,indice));
        fitnessCalculation.Enqueue(CalculDistance(this.transform.position + new Vector3(0, 0, 4),gen,indice));
        fitnessCalculation.Enqueue(CalculDistance(this.transform.position + new Vector3(0, 0, -4),gen,indice));
        //fitnessCalculation.Enqueue( CalculDistance( this.transform.position + new Vector3(10, 0, 0)));
        bestDistance = 0;
    }

    // Start is called before the first frame update
    public void Initialize(Genetic gen,int indice)
    {
        firstLimb = CG.firstLimb;
        fitnessCalculation = new Queue<IEnumerator>();
        Random.InitState(((int)System.DateTime.Now.Ticks));
        input.intputs = new List<float>();
        input.output = new List<float>();

        Vector3 d = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10)).normalized;
        
        input.intputs.Add( d.x);
        input.intputs.Add( d.y);
        input.intputs.Add( d.z);
    
        input.intputs.Add( this.firstLimb.transform.up.x);
        input.intputs.Add( this.firstLimb.transform.up.y);
        input.intputs.Add( this.firstLimb.transform.up.z);
        
        for (int j = 0; j < CG.mov.Count; j++)
        {
            Vector3 dir = (CG.mov[j].transform.position - this.firstLimb.transform.position).normalized;
            input.intputs.Add(dir.x);
            input.intputs.Add(dir.y);
            input.intputs.Add(dir.z);
        }

        for (int j = 0; j < CG.mov.Count; j++)
        {
            Vector3 dir = new Vector3(Random.Range(-1, 1),Random.Range(-1, 1),Random.Range(-1, 1));
            input.output.Add(dir.x);
            input.output.Add(dir.y);
            input.output.Add(dir.z);
        }

        nn = new NN(input.intputs.Count, (input.output.Count + input.intputs.Count), input.output.Count);
        List<Inputs> lst = new List<Inputs>();
        lst.Add(input);
        //nn.train(lst);
        
        prewarm(gen, indice);
      
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position + new Vector3(10, 0, 0),0.5f);
    }
    

    public IEnumerator fitness(Genetic gen)
    {
        int nb = fitnessCalculation.Count;
        while ( fitnessCalculation.Count > 0)
        {
            var c = fitnessCalculation.Dequeue();
            yield return StartCoroutine(c);
        }

        bestDistance = (bestDistance / (float)nb);
        gen.nbCorotine--;
    }

    private void Update()
    {
     
    }

    IEnumerator CalculDistance(Vector3 position,Genetic gen,int indice)
    {

        int idx = 0;
        gen.Respawn(indice);
        
        while (idx <500)
        {
            Inputs i = new Inputs();
            i.intputs = new List<float>();
         
            Vector3 dir = (position - this.firstLimb.transform.position).normalized;
            i.intputs.Add(dir.x);
            i.intputs.Add(dir.y);
            i.intputs.Add(dir.z);
            
    
            i.intputs.Add(this.firstLimb.transform.up.x);
            i.intputs.Add(this.firstLimb.transform.up.y);
            i.intputs.Add(this.firstLimb.transform.up.z);
            
            for (int j = 0; j < CG.mov.Count; j++)
            {
                dir = (CG.mov[j].transform.position - this.firstLimb.transform.position).normalized;
                i.intputs.Add(dir.x);
                i.intputs.Add(dir.y);
                i.intputs.Add(dir.z);
               
           
            }

            List<float> d2 = nn.Update(i.intputs);
            idx++;
            int k = 0;
            for (int j = 0; j < CG.mov.Count; j++)
            {
                Vector3 d = new Vector3(d2[k++], d2[k++], d2[k++]);
                CG.mov[j].Move(d);
            }

            yield return new WaitForFixedUpdate();
        }
       
        bestDistance  += Vector3.Distance(firstLimb.transform.position, position);

    }

}
