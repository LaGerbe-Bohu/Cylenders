using UnityEngine;

public struct Boids
{
    public Vector3 direction;
    public Vector3 position;

    public Vector3 GetDirection() { return this.direction; }
    public Vector3 GetPosition() { return this.position; }
    public void SetDirection(Vector3 dir) {  this.direction =dir ; }
    public void SetPosition(Vector3 pos) {  this.position =pos ; }
    
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
    public ComputeShader computerShader;
    
    [Space]
    public GameObject prefab;
    public float radius;
    
    private Rigidbody[] lstGameObject;
    private Boids[] lstBoids;
    Boids[] data;
    private MobInput[] lstInputs;
    
    // Start is called before the first frame update
    void Start()
    {
        
        
        lstGameObject = new Rigidbody[nbMob];
        lstBoids = new Boids[nbMob];
        lstInputs = new MobInput[nbMob];
        data = new Boids[nbMob];
        for (int i = 0; i < nbMob; i++)
        {
            GameObject go = Instantiate(prefab, this.transform.position, Quaternion.identity);
            go.transform.SetParent(this.transform);
            
            lstBoids[i] = new Boids();
            lstBoids[i].SetPosition(this.transform.position + new Vector3(Random.Range(-radius,radius),Random.Range(0,74f),Random.Range(-radius,radius)));
            lstBoids[i].SetDirection(new Vector3(Random.Range(-1f,1f),Random.Range(-1f,1f),Random.Range(-1f,1f)));
            lstGameObject[i] = go.GetComponent<Rigidbody>();
            data[i] = lstBoids[i];
        }
    }

    Vector3 Target(Boids b)
    {
        var position = this.targePlayer.position;
        return ( new Vector3(position.x,position.y,position.z) - b.GetPosition())/coefTarget;
    }
    
    Vector3 Rule1(Boids b,int k)
    {
        Vector3 pcj = Vector2.zero;
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
    
    Vector3 Rule3(Boids b, int k)
    {
        
        if(lstBoids.Length -1  <= 0) return Vector3.zero;
        
        Vector3 pvj = Vector2.zero;
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
    
    void moveGPU()
    {
        int size = sizeof(float)*3*2;
        ComputeBuffer boidsBuffer = new ComputeBuffer(this.data.Length,size );
        boidsBuffer.SetData(data);
        
        computerShader.SetBuffer(0,"rBoids",boidsBuffer);
        
        computerShader.SetFloat("count",data.Length);
        computerShader.SetFloat("distanceApproach",distanceApproach);
        computerShader.SetFloat("coeffAlligment",coeffAlligment);
        computerShader.SetFloat("spd",Spd);
        computerShader.SetFloat("deltaTime",Time.deltaTime);
        computerShader.SetFloat("sizemax",radius);
        computerShader.SetFloat("distanceSeparation",distanceSeperation);
        computerShader.SetFloat("coeffRapprochement",coeffRapprochement);
        computerShader.SetFloats("transformPos",new float[3]{transform.position.x,transform.position.y,transform.position.z});
        
        int threadGroups = Mathf.CeilToInt (data.Length / (float) 1024f);
        computerShader.Dispatch(0,threadGroups,1,1);
        
        boidsBuffer.GetData(data);
        
        for (int i = 0; i < lstBoids.Length; i++)
        {
            lstBoids[i] = data[i];

           
            lstGameObject[i].transform.position = lstBoids[i].GetPosition();
            lstGameObject[i].transform.rotation = Quaternion.LookRotation(lstBoids[i].GetDirection(),lstGameObject[i].transform.forward);
            
        }

        boidsBuffer.Dispose();

    }
    
    private void computeCPU()
    {
        for (int i = 0; i < lstBoids.Length; i++)
        {
            if (lstGameObject[i] != null )
            {
                
                Vector3 v1 = Rule1(lstBoids[i],i);
                Vector3 v2 = Rule1(lstBoids[i],i);
                Vector3 v3 = Rule1(lstBoids[i],i);
            
                Vector3 v4 = BoundRule(lstBoids[i]);
              

                Vector3 finaldir = lstBoids[i].GetDirection()+v1+v2+v3 + v4;
                
                lstBoids[i].SetDirection(finaldir.normalized * Spd* Time.deltaTime);
                lstBoids[i].SetPosition(lstBoids[i].GetPosition() + lstBoids[i].GetDirection());
            
                lstGameObject[i].transform.position = lstBoids[i].GetPosition();
                lstGameObject[i].transform.rotation = Quaternion.LookRotation(lstBoids[i].GetDirection(),lstGameObject[i].transform.forward);
            }
            
        }
    }
    

    
    // Update is called once per frame
    void Update()
    {
        moveGPU();
    }
}
