using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SnapData
{
    [HideInInspector]
    public bool snapped;
    public Transform transform;
    [SerializeField]
    private Vector3 normal;

    public Vector3 GetNormal()
    {
      return this.transform.TransformDirection(normal);
    }
    
    public void SetNormal(Vector3 norm)
    {
        this.normal = norm;
    }
    
}


public class LimbEplacement : MonoBehaviour
{
    public List<SnapData> snapData;
    private bool find = false;
    private void OnEnable()
    {
       
    }
    
    private void OnDrawGizmos()
    {
        for (int i = 0; i < snapData.Count; i++)
        {
            Gizmos.color = Color.green;
            if (snapData[i].snapped)
            {
                Gizmos.color = Color.red;
            }

            Gizmos.DrawSphere(snapData[i].transform.position,.05f);
            Gizmos.color = Color.black;
            Gizmos.DrawRay(snapData[i].transform.position, snapData[i].GetNormal());
            
        }
    }


    private void Update()
    {
        
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (this.CompareTag("Arm"))
            {
              
            }
        }

        
    }
}
