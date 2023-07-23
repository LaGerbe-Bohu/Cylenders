using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class StructuresManager : MonoBehaviour
{
    public List<Transform> lstAnchors;
    [SerializeField]
    private Transform OriginBounding;
    [SerializeField]
    private float HeightCheck;
    [SerializeField]
    private float DistanceBounding;
    [SerializeField]
    private float radiusCheck = .5f;
    
    
    
    
    public LayerMask LM;
    public float scale;
    List<Vector3> Vec = new List<Vector3>();
    
    [Header("Debug")]
    public int counterRay;

    public float getDistanceBounding()
    {
        return DistanceBounding * scale;
    }
    
    public float getHeightCheck()
    {
        return HeightCheck * scale;
    }
    
    public float getRadius()
    {
        return radiusCheck * scale;
    }
    
    private void OnDrawGizmos()
    {
        
        
        Gizmos.color = Color.yellow;
        Circle.DrawGizmoDisk(OriginBounding,getDistanceBounding());
        
        
        for (int i = 0; i < lstAnchors.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(lstAnchors[i].transform.position,getRadius());
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(lstAnchors[i].transform.position + Vector3.down*getHeightCheck(),getRadius());



            Gizmos.color = Color.green;
            Gizmos.DrawLine(lstAnchors[i].transform.position,lstAnchors[i].transform.position + Vector3.down*getHeightCheck());
            
        }
    }


    public bool findPlace(List<StructuresManager> lstStructures)
    {
        
        bool trouver = false;
        int idx = 0;
        while (!trouver && idx < 1000)
        {
            idx++;
            trouver = true;
       
            Vector3 randompos = Random.insideUnitCircle * GameManager.instance.CylenderRadius;
            RaycastHit hit;
            
            
            if (Physics.Raycast(new Vector3(randompos.x, 10, randompos.y), Vector3.down, out hit, 1000f, LM))
            {
                this.transform.position = new Vector3(randompos.x, hit.point.y, randompos.y);
            }
            
            
            for (int i = 0; i < lstStructures.Count; i++)
            {
                
                if (Vector3.Distance(lstStructures[i].transform.position , this.transform.position) < getDistanceBounding() + lstStructures[i].getDistanceBounding() && lstStructures[i].transform != this.transform)
                {
                    trouver = false;
                  
                }
            }


            if (!trouver)
                continue;
            
            for (int i = 0; i < lstAnchors.Count; i++)
            {

                RaycastHit[] hits =  Physics.SphereCastAll(lstAnchors[i].transform.position + Vector3.down * getHeightCheck(), getRadius(),
                    Vector3.down,
                    Mathf.Infinity, LM);


                for (int j = 0; j < hits.Length; j++)
                {
                    if (hits[j].transform != this.transform)
                    {
                        trouver = false;
                        break;
                    } 
                }

                if (!trouver)
                    break;
                
                hits = Physics.SphereCastAll(lstAnchors[i].transform.position, getRadius(), Vector3.down,
                    Mathf.Infinity, LM);

                bool tx = false;
                for (int j = 0; j < hits.Length; j++)
                {
                    if (hits[j].transform != this.transform)
                    {
                        tx = true;
                    }
                }

                if (!tx)
                {
                    trouver = false;
                    break;
                }

            }
      
         
        }
        
        
        return trouver;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    
    
}
