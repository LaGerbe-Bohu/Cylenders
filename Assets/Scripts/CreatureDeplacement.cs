using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    private bool start = false;
    private Queue<IEnumerator> fitnessCalculation;
    Inputs input = new Inputs();
    // Start is called before the first frame update
    void Start()
    {
        firstLimb = CG.firstLimb;
        fitnessCalculation = new Queue<IEnumerator>();

        input.intputs = new List<float>();
        input.output = new List<float>();
        input.intputs.Add( this.firstLimb.transform.up.x);
        input.intputs.Add( this.firstLimb.transform.up.y);
        input.intputs.Add( this.firstLimb.transform.up.z);
        
        input.intputs.Add( this.firstLimb.transform.position.x);
        input.intputs.Add( this.firstLimb.transform.position.y);
        input.intputs.Add( this.firstLimb.transform.position.z);       
        
        input.intputs.Add( target.transform.position.x);
        input.intputs.Add( target.transform.position.y);
        input.intputs.Add( target.transform.position.z);

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
        
        fitnessCalculation.Enqueue( CalculDistance(new Vector3(4, 0, 0))); 
        fitnessCalculation.Enqueue( CalculDistance(new Vector3(-4, 0, 0))); 
        fitnessCalculation.Enqueue( CalculDistance(new Vector3(0, 0, 4 ))); 
        fitnessCalculation.Enqueue( CalculDistance(new Vector3(0, 0, -4)));
    }

    // Update is called once per frame
    void Update()
    {
           
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
        
      
     
    // fitness();
    }


    public void fitness()
    {
        if (!start && fitnessCalculation.Count > 0 )
        {
        
            StartCoroutine(    fitnessCalculation.Dequeue());
        }
        
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
        start = true;
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

            yield return new WaitForSeconds(1f);
        }

        start = false;
        bestDistance  += Vector3.Distance(firstLimb.transform.position, position);
        Debug.Log(bestDistance);

    }

}
