using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public struct Boids
{
    private Vector3 direction;
    private Vector3 position;

    public void SetDirection(Vector3 v)
    {
        this.direction = v;
    }

    public void SetPosition(Vector3 p)
    {
        this.position = p;
    }


    public Vector3 GetPosition()
    {
        return this.position;
    }

    public Vector3 GetDirection()
    {
        return this.direction;
    }
    
}

public class MobManager : MonoBehaviour
{

    [Header("Values")] 
    public int nbMob;
    public float distanceApproach;
    public float distanceSeperation;
    public float coeffRapprochement;
    public float coeffAlligment;
    public float Spd;
    
    [Space]
    public GameObject prefab;
    public float radius;
    
    private GameObject[] lstGameObject;
    private Boids[] lstBoids;
    
    
    // Start is called before the first frame update
    void Start()
    {
        lstGameObject = new GameObject[nbMob];
        lstBoids = new Boids[nbMob];
        
        for (int i = 0; i < nbMob; i++)
        {
            GameObject go = Instantiate(prefab, this.transform.position, Quaternion.identity);
            go.transform.SetParent(this.transform);
            
            lstBoids[i] = new Boids();
            lstBoids[i].SetPosition(new Vector3(Random.Range(-radius,radius),Random.Range(0,74f),Random.Range(-radius,radius)));
            lstBoids[i].SetDirection(new Vector3(Random.Range(-1f,1f),Random.Range(-1f,1f),Random.Range(-1f,1f)));
            
            lstGameObject[i] = go;
        }
    }

    Vector3 rule1(Boids b,int k)
    {
        Vector3 pcj = Vector3.zero;
        int sum = 0;
        for (int i = 0; i < lstBoids.Length; i++)
        {
            if (i != k)
            {
                
                if (Vector3.Distance(b.GetPosition(), lstBoids[i].GetPosition()) < distanceApproach)
                {
                    pcj += lstBoids[i].GetPosition();
                    sum++;
                }
            }
        }
        
        if(sum == 0) return  Vector3.zero;
        pcj = pcj / (sum);
        return (pcj - b.GetPosition()) / coeffRapprochement ;
    }
    
    Vector3 rule2(Boids b, int k)
    {
        Vector3 c = Vector3.zero;
        
        for (int i = 0; i < lstBoids.Length; i++)
        {
            if (i != k)
            {
       
                if (Vector3.Distance(b.GetPosition(), lstBoids[i].GetPosition()) < distanceSeperation)
                {
                    c = c - ( lstBoids[i].GetPosition() - b.GetPosition() );
                }
            }
        }

        return c;
    }
    
    Vector3 rule3(Boids b, int k)
    {
        
        if(lstBoids.Length -1  <= 0) return Vector3.zero;
        
        Vector3 pvj = Vector3.zero;
        int sum = 0;
        for (int i = 0; i < lstBoids.Length; i++)
        {
            if (i != k)
            {
                if (Vector3.Distance(b.GetPosition(), lstBoids[i].GetPosition()) < distanceApproach)
                {
                    pvj += lstBoids[i].GetDirection();
                    sum++;
                }
            }
        }

        if (sum == 0) return Vector3.zero;
        
        pvj = pvj / (sum);
        
        Vector3 v = (pvj - b.GetDirection()) / coeffAlligment;
        
        
        return v;
    }
    
    Vector3 boundRule(Boids b)
    {
        Vector3 v = Vector3.zero;
        
        if (b.GetPosition().x < -radius)
        {
            v.x = 1;
        }

        if (b.GetPosition().x > radius)
        {
            v.x = -1;
        } 
        
        if (b.GetPosition().y < -43f)
        {
            v.y = 1;
        }

        if (b.GetPosition().y > 74f)
        {
            v.y = -1;
        } 
        

        if (b.GetPosition().z < -radius)
        {
            v.z = 1;
        }

        if (b.GetPosition().z > radius)
        {
            v.z = -1;
        }

        return v/100.0f;
    }
    private void computeCPU()
    {
        for (int i = 0; i < lstBoids.Length; i++)
        {
            
            Vector3 v1 = rule1(lstBoids[i], i);
            Vector3 v2 = rule2(lstBoids[i], i);
            Vector3 v3 = rule3(lstBoids[i], i);
            Vector3 v4 = boundRule(lstBoids[i]);
            
            
            Vector3 finalDir = (lstBoids[i].GetDirection() + v1 + v2 + v3 + v4).normalized*Spd * Time.deltaTime;
            
            lstBoids[i].SetDirection(finalDir);
            lstBoids[i].SetPosition( lstBoids[i].GetPosition() + finalDir);

            if (lstGameObject[i] != null)
            {
                lstGameObject[i].transform.position = lstBoids[i].GetPosition();
                lstGameObject[i].transform.rotation = Quaternion.LookRotation(lstBoids[i].GetDirection(),lstGameObject[i].transform.forward);    
            }
            
        }
    }
    

    
    // Update is called once per frame
    void Update()
    {
        computeCPU();
    }
}
