using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

public enum LimbType
{
   
    Arm ,
    Body,
    Articulation,
    Liaison
}

public class Limb
{
    public LimbType type;
    public LimbEplacement limpEmplacement;
    public Transform transform;
    public List<Limb> next;
    public int NbLimb;
   
    
    public Limb()
    {
           
        Array values = Enum.GetValues(typeof(LimbType));
        type = (LimbType) values.GetValue(Random.Range(0,values.Length));

        do
        {
            values = Enum.GetValues(typeof(LimbType));
            type = (LimbType) values.GetValue(Random.Range(0, values.Length));
        } while (type == LimbType.Body);

        NbLimb = CreaturesGenerator.getNext(type);
        next = new List<Limb>();
    }
    
    public Limb(LimbType t)
    {
        type = t;
        NbLimb = CreaturesGenerator.getNext(type);
        next = new List<Limb>();
    }

    public Limb(LimbType[] expect)
    {
        Array values = Enum.GetValues(typeof(LimbType));
        type = (LimbType) values.GetValue(Random.Range(0,values.Length));

        do
        {
            values = Enum.GetValues(typeof(LimbType));
            type = (LimbType) values.GetValue(Random.Range(0, values.Length));
        } while (!expect.Contains(type));

        NbLimb = CreaturesGenerator.getNext(type);
        next = new List<Limb>();
    }
    
}


[Serializable]
public struct LimbValue
{
    public LimbType type;
    public float rate;
    public GameObject Object;
    public int nbNext;
}

public class CreaturesGenerator : MonoBehaviour
{
    public List<LimbValue> ListValue;
    public static List<LimbValue> sListValue;
    public LimbType firstType;
    public int Height;
    private Limb firstLimb;
    private static readonly float Phi = Mathf.PI * (3.0f - Mathf.Sqrt(5.0f));
    [HideInInspector] 
    public Rigidbody RBofCreature;

    public CreatureMovement tempLaison;
    public void Start()
    {
        sListValue = ListValue;
        firstLimb = new Limb(firstType);
        
        firstLimb.transform = Instantiate(getObject(firstLimb.type), this.transform.position,Quaternion.identity).transform;
        firstLimb.limpEmplacement = firstLimb.transform.GetComponent<LimbEplacement>();
        firstLimb.NbLimb = firstLimb.limpEmplacement.snapData.Count;
        firstLimb.transform.SetParent(this.transform);
        RBofCreature = firstLimb.transform.GetComponent<Rigidbody>();
        CreateCreature(firstLimb,Height);
        GenerateCreature(firstLimb);

    
    }

    public GameObject getObject(LimbType v) 
    {
        for (int i = 0; i < ListValue.Count; i++)
        {
            if (ListValue[i].type == v)
            {
                return ListValue[i].Object;
            }
        }
        return null;
    }
    
    public float getObjetRate(LimbType v) 
    {
        for (int i = 0; i < ListValue.Count; i++)
        {
            if (ListValue[i].type == v)
            {
                return ListValue[i].rate;
            }
        }

        return 0;
    }

    public static int getNext(LimbType v)
    {
        for (int i = 0; i < sListValue.Count; i++)
        {
            if (sListValue[i].type == v)
            {
                return sListValue[i].nbNext;
            }
        }

        return 0;
    }
    
    public void GenerateCreature(Limb L)
    {
        if (L == null) return;

        if (L.transform.CompareTag("Articulation"))
        {
            tempLaison = L.transform.GetComponent<CreatureMovement>();
    
        }
        
        for (int i = 0; i < L.next.Count; i++)
        {
            
            if (L.limpEmplacement.snapData[i].snapped) continue;
            
  
            MeshRenderer b = getObject(L.next[i].type).GetComponent<MeshRenderer>();
            L.next[i].transform = Instantiate(getObject(L.next[i].type),this.transform).transform;
            L.next[i].limpEmplacement = L.next[i].transform .GetComponent<LimbEplacement>();
            L.next[i].transform.rotation = Quaternion.LookRotation(-L.limpEmplacement.snapData[i].GetNormal(),L.next[i].limpEmplacement.snapData[0].GetNormal());
            L.next[i].transform.position = L.limpEmplacement.snapData[i].transform.position;
            Vector3 offset =   L.next[i].limpEmplacement.snapData[0].transform.position -  L.limpEmplacement.snapData[i].transform.position ;
            L.next[i].transform.position -= offset;
            L.limpEmplacement.snapData[i].snapped = true;
            L.next[i].limpEmplacement.snapData[0].snapped = true;

            if (L.next[i].type == LimbType.Liaison && tempLaison !=null || L.next[i].type == LimbType.Articulation && tempLaison !=null)
            {
                tempLaison.arms.Add(L.next[i].transform.GetComponent<HingeJoint>());
            }
            
            
            Joint Hj = L.next[i].transform.GetComponent<Joint>();
           
            Hj.connectedBody = L.transform.GetComponent<Rigidbody>();
          
        
            
             GenerateCreature(L.next[i]);
             
        }
        
    }
    
    public void CreateCreature(Limb L,int H)
    {



        if (H <= 0 && L.type != LimbType.Arm)
        {
            L.NbLimb++;
            L.next = new List<Limb>();
            L.next.Add(new Limb(LimbType.Arm));
            return;
        }
       
        for (int i = 0; i < L.NbLimb; i++)
        {
        
            if (L.type == LimbType.Articulation)
            {
                L.next.Add(new Limb(LimbType.Arm));
                
            }
            else
            {
                if (L.type == LimbType.Arm && getObjetRate(LimbType.Articulation) > Random.Range(0f, 1f))
                {
                    L.next.Add(new Limb(LimbType.Articulation));
                }
                else if(L.type == LimbType.Arm)
                {
                    L.next.Add(new Limb(LimbType.Liaison));
                }
                else
                {
                    L.next.Add(new Limb(LimbType.Arm));
                }
            }
         
                
            CreateCreature(L.next[i], H-1);
        }
        
    }
    
    public void DisplayCreature(Limb L)
    {
        Debug.Log(L.type);
        
        for (int i = 0; i < L.NbLimb; i++)
        {
            DisplayCreature(L.next[i]);
            
            
        }
    }
    
    Vector3 fibonacci_sphere(int N,int maxLimb)
    {
        float y = 1 - (N / (float)(maxLimb - 1)) * 2;
        float radius = Mathf.Sqrt(1 - y * y);
    
        float theta = Phi * N;
        float x = Mathf.Cos(theta) * radius;
        float z = Mathf.Sin(theta) * radius;
        return new Vector3(x, z, y);
    }
    
    
    
    /*
    
    public MeshRenderer startBody;
    public MeshRenderer[] Articulates;
    public MeshRenderer[] Limb;
    public int maxLimb = 2;
   
   
    
    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < maxLimb; i++)
        {
            
            Vector3 pos = fibonacci_sphere(i);
            pos = this.transform.TransformPoint(pos);
            Vector3 dir = pos - this.transform.position;
            Gizmos.color = Color.red;
            Gizmos.DrawRay(pos,dir*10f);
          
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Draw());
    }

    IEnumerator Draw()
    {
        for (int i = 0; i < maxLimb; i++)
        {
            
            Vector3 pos = fibonacci_sphere(i);
            Vector3 global_Pos = this.transform.TransformPoint(pos);
            Mesh mesh = GetComponent<MeshFilter>().mesh;
            Vector3 [] vertices = mesh.vertices;

            float bestValue = -1;
            int bestIndice = 0;
            float bestDistance = 100000f;
            
            for (int j = 0; j < vertices.Length; j++)
            {
                float dot = Vector3.Dot(vertices[j],pos.normalized);
                float distance = Vector3.Distance(vertices[j], pos.normalized);
               
                if ( dot >= bestValue && distance < bestDistance )
                {
                    bestValue = dot;
                    bestIndice = j;
                    bestDistance = distance;
                    
                }
                
            }

            Vector3 dir = global_Pos - this.transform.position;
            dir = dir.normalized;

            MeshRenderer NextLimb;

            if (this.CompareTag("Articulation"))
            {
                int random =Random.Range(0,Limb.Length);
                NextLimb = Limb[random];
            }
            else
            {
                int random =Random.Range(0,Articulates.Length);
                NextLimb = Articulates[random];
            }

            Vector3 target_pos = this.transform.TransformPoint(vertices[bestIndice]+dir*NextLimb.bounds.size.y);
            
            if (!Physics.Raycast(target_pos, dir , NextLimb.bounds.size.y))
            {
                GameObject go = Instantiate(NextLimb.gameObject);
                go.transform.position = this.transform.TransformPoint(vertices[bestIndice])+dir*NextLimb.bounds.extents.y;
                go.transform.rotation = Quaternion.LookRotation(dir);
            }
        
            yield return new WaitForSeconds(.6f);
        }
    }
    */    

    // Update is called once per frame
    void Update()
    {
        
    }
}
