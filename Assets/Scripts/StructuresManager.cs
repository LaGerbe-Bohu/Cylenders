using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;


public class StructuresManager : MonoBehaviour
{
    public List<Transform> lstAnchors;
    public Transform OriginBounding;
    public float HeightCheck;
    public float DistanceBounding;
  
    public LayerMask LM;

    List<Vector3> Vec = new List<Vector3>();
    
    [Header("Debug")]
    public int counterRay;
    

    private void OnDrawGizmos()
    {
        
        for (float i = 0.0f; i < 2*Mathf.PI; i+=(2*Mathf.PI)/(float)(counterRay>=1 ? counterRay : 1.0f ))
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(OriginBounding.position,new Vector3(Mathf.Cos(i),0,Mathf.Sin(i) )*DistanceBounding);
            
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



    public int findPlace(List<Transform> lstStructures)
    {
        
        bool trouver = false;
        int idx = 0;
        while (!trouver && idx < 300)
        {
          
            trouver = true;
       
            Vector3 randompos = Random.insideUnitCircle * GameManager.instance.CylenderRadius;
            RaycastHit hit;
      
            if (Physics.Raycast(new Vector3(randompos.x, 10, randompos.y), Vector3.down, out hit, 1000f, LM))
            {
                this.transform.position = new Vector3(randompos.x, hit.point.y, randompos.y);
            }

            idx++;
            
            for (int i = 0; i < lstStructures.Count; i++)
            {
                if (Vector3.Distance(lstStructures[i].position, this.transform.position) < DistanceBounding  && lstStructures[i] != this.transform)
                {
                    trouver = false;
                    Debug.Log("touche");
                }
            }

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
      
         
        }
        
   
        if (!trouver || idx >= 300)
        {
            Destroy(this.gameObject);
        }

        return idx;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    
    
}
