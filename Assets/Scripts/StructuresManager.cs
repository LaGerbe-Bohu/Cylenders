using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class StructuresManager : MonoBehaviour
{
    public List<Transform> lstAnchors;
    public Transform OriginBounding;
    public float HeightCheck;
    public float DistanceBounding;
    public float radiusCheck = .5f;
    public LayerMask LM;

    List<Vector3> Vec = new List<Vector3>();
    
    [Header("Debug")]
    public int counterRay;
    
    private void OnDrawGizmos()
    {
        
        
        Gizmos.color = Color.yellow;
        Circle.DrawGizmoDisk(OriginBounding,DistanceBounding);
        
        
        for (int i = 0; i < lstAnchors.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(lstAnchors[i].transform.position,radiusCheck);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(lstAnchors[i].transform.position + Vector3.down*HeightCheck,radiusCheck);



            Gizmos.color = Color.green;
            Gizmos.DrawLine(lstAnchors[i].transform.position,lstAnchors[i].transform.position + Vector3.down*HeightCheck);
            
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
                
                if (Vector3.Distance(lstStructures[i].transform.position , this.transform.position) < DistanceBounding + lstStructures[i].DistanceBounding && lstStructures[i].transform != this.transform)
                {
                    trouver = false;
                
                }
            }

            for (int i = 0; i < lstAnchors.Count; i++)
            {

                if (Physics.SphereCast(lstAnchors[i].transform.position + Vector3.down * HeightCheck, radiusCheck, Vector3.down,
                    out hit, Mathf.Infinity, LM))
                {
                    trouver = false;
                    break;
                }

                if (!Physics.SphereCast(lstAnchors[i].transform.position, radiusCheck, Vector3.down, out hit, Mathf.Infinity, LM))
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
