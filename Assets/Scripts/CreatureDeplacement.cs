using System;
using System.Collections;
using System.Collections.Generic;
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
    private Queue<IEnumerator> TmpfitnessCalculation;
    Inputs input = new Inputs();


    private void OnJointBreak(float breakForce)
    {
        foreach (Transform tr in this.transform)
        {
            Destroy(tr.gameObject);
        }


        Debug.Log("BREAK");
        CG.Generator(CG.seed);
        this.firstLimb = CG.firstLimb;
    }

    private void Start()
    {
      
        Initialize();
    }

    // Start is called before the first frame update
    public void Initialize()
    {
        firstLimb = CG.firstLimb;
        fitnessCalculation = new Queue<IEnumerator>();
        Random.InitState((int)System.DateTime.Now.Ticks);
        input.intputs = new List<float>();
        input.output = new List<float>();
        input.intputs.Add( this.firstLimb.transform.up.x);
        input.intputs.Add( this.firstLimb.transform.up.y);
        input.intputs.Add( this.firstLimb.transform.up.z);
        
        input.intputs.Add( this.firstLimb.transform.position.x);
        input.intputs.Add( this.firstLimb.transform.position.y);
        input.intputs.Add( this.firstLimb.transform.position.z);

 
        input.intputs.Add( Random.Range(-10, 10));
        input.intputs.Add( Random.Range(-10, 10));
        input.intputs.Add( Random.Range(-10, 10));

        for (int j = 0; j < CG.mov.Count; j++)
        {
            Vector3 dir = new Vector3(Random.Range(-1, 1),Random.Range(-1, 1),Random.Range(-1, 1));
            input.output.Add(dir.x);
            input.output.Add(dir.y);
            input.output.Add(dir.z);
        }
        
        nn = new NN(input.intputs.Count, 5, input.output.Count);
        List<Inputs> lst = new List<Inputs>();
        lst.Add(input);
        nn.train(lst);
        
        fitnessCalculation.Enqueue( CalculDistance( this.transform.position + new Vector3(4, 0, 0))); 
        fitnessCalculation.Enqueue( CalculDistance(this.transform.position + new Vector3(-4, 0, 0))); 
        fitnessCalculation.Enqueue( CalculDistance(this.transform.position + new Vector3(0, 0, 4 ))); 
        fitnessCalculation.Enqueue( CalculDistance(this.transform.position + new Vector3(0, 0, -4)));

        TmpfitnessCalculation = new Queue<IEnumerator>(fitnessCalculation);
        
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
   
        
        
    }

    // Update is called once per frame
    void Update()
    {
           
        /*
        timer -= Time.deltaTime;

        
        if (timer < 0)
        {
            timer = 1.0f;
            Inputs i = new Inputs();
            i.intputs = new List<float>();
            i.intputs.Add( this.firstLimb.transform.up.x);
            i.intputs.Add( this.firstLimb.transform.up.y);
            i.intputs.Add( this.firstLimb.transform.up.z);
            
            i.intputs.Add( this.firstLimb.transform.position.x);
            i.intputs.Add( this.firstLimb.transform.position.y);
            i.intputs.Add( this.firstLimb.transform.position.z);       
            
            i.intputs.Add( target.transform.position.x);
            i.intputs.Add( target.transform.position.y);
            i.intputs.Add( target.transform.position.z);
            
            List<float> d2 = nn.Update(i.intputs);

            int idx = 0;
            for (int j = 0; j < CG.mov.Count; j++)
            {
                Vector3 d = new Vector3(d2[idx++], d2[idx++], d2[idx++]);
                CG.mov[j].Move(d);
            }
            
            Debug.Log(Vector3.Distance(firstLimb.transform.position, new Vector3(4,0,0)));
        }
        */
   
    }


    public IEnumerator fitness(Genetic gen)
    {

        while ( fitnessCalculation.Count > 0)
        {
            yield return StartCoroutine(    fitnessCalculation.Dequeue());
        }

        gen.nbCorotine--;
        
        fitnessCalculation = new Queue<IEnumerator>(TmpfitnessCalculation);
    }
    
    IEnumerator CalculDistance(Vector3 position)
    {

        int idx = 0;

        foreach (Transform tr in this.transform)
        {
            Destroy(tr.gameObject);
        }

   
        CG.Generator(CG.seed);
        this.firstLimb = CG.firstLimb;

        while (idx < 10)
        {
            Inputs i = new Inputs();
            i.intputs = new List<float>();
            i.intputs.Add(this.firstLimb.transform.up.x);
            i.intputs.Add(this.firstLimb.transform.up.y);
            i.intputs.Add(this.firstLimb.transform.up.z);

            i.intputs.Add(this.firstLimb.transform.position.x);
            i.intputs.Add(this.firstLimb.transform.position.y);
            i.intputs.Add(this.firstLimb.transform.position.z);

            i.intputs.Add(position.x);
            i.intputs.Add(position.y);
            i.intputs.Add(position.z);

            List<float> d2 = nn.Update(i.intputs);
          
            idx++;
            int k = 0;
            for (int j = 0; j < CG.mov.Count; j++)
            {
                Vector3 d = new Vector3(d2[k++], d2[k++], d2[k++]);
                CG.mov[j].Move(d);
            }

            yield return new WaitForSeconds(1);
        }


        bestDistance  += Vector3.Distance(firstLimb.transform.position, position);

    }

}
