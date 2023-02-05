using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public struct Boids
{
    private Vector2 direction;
    private Vector2 position;
    
    public void SetDirection(Vector2 v)
    {
        this.direction = v;
    }

    public void SetPosition(Vector2 p)
    {
        this.position = p;
    }


    public Vector2 GetPosition()
    {
        return this.position;
    }

    public Vector2 GetDirection()
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
    public float coefTarget;
    public Transform targePlayer;
    
    
    [Space]
    public GameObject prefab;
    public float radius;
    
    private Rigidbody[] lstGameObject;
    private Boids[] lstBoids;
    private MobInput[] lstInputs;
    
    // Start is called before the first frame update
    void Start()
    {
        
        
        lstGameObject = new Rigidbody[nbMob];
        lstBoids = new Boids[nbMob];
        lstInputs = new MobInput[nbMob];
        
        for (int i = 0; i < nbMob; i++)
        {
            GameObject go = Instantiate(prefab, this.transform.position, Quaternion.identity);
            go.transform.SetParent(this.transform);
            
            lstBoids[i] = new Boids();
            lstBoids[i].SetPosition(  new Vector3(Random.Range(-radius,radius),Random.Range(0,74f),Random.Range(-radius,radius)));
            lstBoids[i].SetDirection(new Vector3(Random.Range(-1f,1f),Random.Range(-1f,1f),Random.Range(-1f,1f)));

            lstInputs[i] = go.GetComponent<MobInput>();
            lstGameObject[i] = go.GetComponent<Rigidbody>();
        }
    }

    Vector2 Target(Boids b)
    {
        var position = this.targePlayer.position;
        return ( new Vector2(position.x,position.z) - b.GetPosition())/coefTarget;
    }
    
    Vector3 Rule1(Boids b,int k)
    {
        Vector2 pcj = Vector2.zero;
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
    
    Vector3 Rule2(Boids b, int k)
    {
        Vector2 c = Vector3.zero;
        
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
    
    Vector3 Rule3(Boids b, int k)
    {
        
        if(lstBoids.Length -1  <= 0) return Vector3.zero;
        
        Vector2 pvj = Vector2.zero;
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
    
    Vector3 BoundRule(Boids b)
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
        

        return v/100.0f;
    }
    private void computeCPU()
    {
        for (int i = 0; i < lstBoids.Length; i++)
        {
            
            lstBoids[i].SetPosition( new Vector2(lstGameObject[i].position.x,lstGameObject[i].position.z));
            lstBoids[i].SetDirection(new Vector2(lstGameObject[i].velocity.x,lstGameObject[i].velocity.z));
            
           // Vector3 v1 = rule1(lstBoids[i], i);
            Vector2 v2 = Rule2(lstBoids[i], i);
            Vector2 v3 = Rule3(lstBoids[i], i);
            Vector2 v4 = Target(lstBoids[i]);
            
            Vector2 finalDir = (lstBoids[i].GetDirection()  + v2 + v3 + v4).normalized;
            
            lstInputs[i].setDirection(finalDir);
           // lstGameObject[i].AddForce(new Vector3(finalDir.x,0,finalDir.y),ForceMode.Impulse);
        }
    }
    

    
    // Update is called once per frame
    void Update()
    {
        computeCPU();
    }
}
