using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;


public class StructuresManager : MonoBehaviour
{
    public List<Transform> lstAnchors;
    public Transform OriginBounding;
    public float HeightCheck;
    public float StartDistanceBounding;
    public float HeightBounding;
    public float CounterRay;
    public LayerMask LM;
  
    
    List<Vector3> Vec = new List<Vector3>();
    
    private void OnDrawGizmos()
    {

        LoadVec();
       
        for (int i = 0; i < Vec.Count; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(OriginBounding.position + Vec[i]*StartDistanceBounding,Vec[i]*HeightBounding);
            
        }
        
        for (int i = 0; i < lstAnchors.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(lstAnchors[i].transform.position,.5f);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(lstAnchors[i].transform.position + Vector3.down*HeightCheck,.5f);



            Gizmos.color = Color.green;
            Gizmos.DrawLine(lstAnchors[i].transform.position,lstAnchors[i].transform.position + Vector3.down*HeightCheck);
            
        }
    }

    void LoadVec()
    {
         Vec = new List<Vector3>();

        for (float i = 0.0f; i < 2*Mathf.PI*CounterRay; i+=(2*Mathf.PI)/CounterRay)
        {
            Vec.Add(new Vector3(Mathf.Cos(i),0,Mathf.Sin(i) ));
        }
        
        
    }
    
    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(findPlace());

    }

    IEnumerator findPlace()
    {
        LoadVec();
        
        bool trouver = false;
        int idx = 0;
        while (!trouver && idx < 300)
        {
            this.gameObject.layer = LayerMask.NameToLayer("Default");
            trouver = true;
       
            Vector3 randompos = Random.insideUnitCircle * GameManager.instance.CylenderRadius;
            RaycastHit hit;
      
            if (Physics.Raycast(new Vector3(randompos.x, 10, randompos.y), Vector3.down, out hit, 1000f, LM))
            {
                this.transform.position = new Vector3(randompos.x, hit.point.y, randompos.y);
            }

            idx++;
            for (int i = 0; i < Vec.Count; i++)
            {
                
                if(Physics.SphereCast(OriginBounding.position + Vec[i]*StartDistanceBounding,2f,Vec[i],out hit,HeightBounding,LM))
                {
                    trouver = false;
                    Debug.Log("touche");
               
                    break;
                }
            }

            this.gameObject.layer = LayerMask.NameToLayer("GroundLayer");
                
            for (int i = 0; i < lstAnchors.Count; i++)
            {

                if (Physics.SphereCast(lstAnchors[i].transform.position + Vector3.down * HeightCheck, .5f, Vector3.down,
                    out hit, Mathf.Infinity, LM))
                {
                    trouver = false;
                    break;
                }

                if (!Physics.SphereCast(lstAnchors[i].transform.position, .5f, Vector3.down, out hit, Mathf.Infinity, LM))
                {
                    trouver = false;
                    break;
                }

            }
            
       
            yield return new WaitForFixedUpdate();
        }
        
   
        if (!trouver || idx >= 300)
        {
         
            Destroy(this.gameObject);
         
        }
        
        
    }
    
    
    // Update is called once per frame
    void Update()
    {
        
    }
    
    
}
