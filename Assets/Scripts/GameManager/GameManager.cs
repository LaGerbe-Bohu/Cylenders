using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;



public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton qui me permettra d'accéder à certiaines variables n'importe d'où dans le code
    /// </summary>
    /// 
    public static GameManager instance;

    public int Seed;
    [HideInInspector]
    public CharacterInput characterInput;
    public float PlayerReach = 2f;
    public List<GenerationPreset> LstGenerationPreset;
    [SerializeField] private Renderer cylenderRender;
    public GameObject PlayerPrefab;

    public LootManager LootManager;
    public Transform sawner;
    public MobSpawner mobSpawner;

    public List<float> levelWeaponRate;

    //public field
    [HideInInspector]
    [FormerlySerializedAs("Player")] public Transform player;
    [HideInInspector]
    [FormerlySerializedAs("camera")] public Transform cameraPlayer;


    public List<EnnemieInformation> Ennemieslv1;
    public List<EnnemieInformation> Ennemieslv2;
    public List<EnnemieInformation> Ennemieslv3;
    public List<EnnemieInformation> Ennemieslv4;
    

    [HideInInspector]
    public List<EnnemieInformation> Ennemies;
    
    private bool oldBool = false;
    
    [Header("Animation")] 
    public bool InAnimiantion;
    
    [HideInInspector]
    public float CylenderRadius;
    
    public UnityEvent e_PlayerHurt;

    [HideInInspector]
    public float timeStepAnimation = 0;

    [HideInInspector]
    public PlayerInformation PI;

    public float LevelIndice;
    
    private void Awake()
    {
        instance = this;
        CylenderRadius = cylenderRender.bounds.extents.magnitude/2f;

        if (e_PlayerHurt == null)
        {
            e_PlayerHurt = new UnityEvent();
        }
        
        if (!GameObject.FindWithTag("Player"))
        {
            GameObject go = Instantiate(PlayerPrefab);
            this.player = go.transform;
            Object.DontDestroyOnLoad(player.gameObject);
            this.cameraPlayer = this.player.transform.GetChild(1);
            characterInput = this.player.GetComponent<CharacterInput>();
            PI = this.player.GetComponent<PlayerInformation>();
      
            
        }
        else
        {
            GameObject go = GameObject.FindWithTag("Player");
            this.player = go.transform;
            Object.DontDestroyOnLoad(player.gameObject);
            this.cameraPlayer = this.player.transform.GetChild(1);
            characterInput = this.player.GetComponent<CharacterInput>();
            PI = this.player.GetComponent<PlayerInformation>();
            this.player.GetComponent<CharacterView>().Initialize();
         
        }
        
        
        if (sawner != null)
        {
            this.player.position = sawner.transform.position;    
        }
        

        this.player.gameObject.SetActive(true);



        if (Ennemieslv1.Count > 0 && Ennemieslv2.Count > 0  && Ennemieslv3.Count > 0 && Ennemieslv4.Count > 0 )
        {
            float t = Mathf.Clamp( GameManager.instance.PI.Room / 15.0f,0,0.99f);
            float nbMob = Mathf.Lerp(5, 100, Mathf.Clamp01( GameManager.instance.PI.Room / 15.0f));
        
            for (int i = 0; i < nbMob; i++)
            {
                bool find = false;
                while (!find)
                {
                    float a = (1.0f-t)*(1.0f-t)*(1.0f-t);
                    float b = (3.0f*t)*((1.0f-t)*(1.0f-t));
                    float c = ((3.0f*t)*(3.0f*t))*(1.0f-t);
                    float d = (t*t*t);

                    if (Random.Range(0, 1f) < a)
                    {
                        find = true;
                        Ennemies.Add( Ennemieslv1[Random.Range(0, Ennemieslv1.Count)]);
                        break;
                    }
                
                    if (Random.Range(0, 1f) < b)
                    {
                        find = true;
                        Ennemies.Add( Ennemieslv2[Random.Range(0, Ennemieslv2.Count)]);
                        break;
                    }
                
                    if (Random.Range(0, 1f) < c)
                    {
                        find = true;
                        Ennemies.Add( Ennemieslv3[Random.Range(0, Ennemieslv3.Count)]);
                        break;
                    }
                
                    if (Random.Range(0, 1f) < d)
                    {
                        find = true;
                        Ennemies.Add( Ennemieslv4[Random.Range(0, Ennemieslv4.Count)]);
                        break;
                    }
                
                }     
            }
        }
     
        
        
        if (Seed != 0)
        {
            Random.InitState(Seed);    
        }
        else
        {
            int s = Random.Range(0, int.MaxValue);
            Random.InitState(s);
       
        }
        
    }

    private void Update()
    {
        if (InAnimiantion != oldBool && InAnimiantion == true)
        {
            timeStepAnimation = 1;
        }
        
        if (InAnimiantion != oldBool && InAnimiantion == false)
        {
            timeStepAnimation = 0;
        }

        oldBool = InAnimiantion;

        if (oldBool && timeStepAnimation > -100)
        {
            timeStepAnimation -= Time.deltaTime;
        }
        else if(timeStepAnimation < 100)
        {
            timeStepAnimation += Time.deltaTime;
        }

        if (PI.life <= 0)
        {
            SceneManager.LoadScene(0);
        }
        
    }

    public GenerationPreset GetRandomGeneration()
    {
        return LstGenerationPreset[Random.Range(0, LstGenerationPreset.Count)];
    }

    public void StartGame()
    {
        InAnimiantion = true;
        PI.Room++;
        StartCoroutine(SwitchRoom(2));
    }

    public void NextLevel()
    {
        InAnimiantion = true;
    
        int s = SceneManager.GetActiveScene().buildIndex;
        if (true)
        {
            s = 1;
        }
        
        StartCoroutine(SwitchRoom(s));
    }
    
    IEnumerator SwitchRoom(int i)
    {
        while (timeStepAnimation > -1.2f)
        {
            yield return null;
        }
      
        SceneManager.LoadScene(i);
    }

    public void PlayerHurt(float dommage)
    {
        PI.life -= dommage/(float)PI.ArmorValue;
        e_PlayerHurt.Invoke();

    }

}
