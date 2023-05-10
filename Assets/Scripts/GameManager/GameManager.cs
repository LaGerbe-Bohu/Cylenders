using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
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
    public CharacterInput characterInput;
    public int playerLife = 5;
    public float PlayerReach = 2f;
    public List<GenerationPreset> LstGenerationPreset;
    [SerializeField] private Renderer cylenderRender;
    public GameObject PlayerPrefab;
    //public field
    [FormerlySerializedAs("Player")] public Transform player;
    public Transform camera;
    
    [HideInInspector]
    public float CylenderRadius;

    [HideInInspector]
    public UnityEvent e_PlayerHurt;
    private void Awake()
    {
        instance = this;
        CylenderRadius = cylenderRender.bounds.extents.magnitude/2f;

        if (!GameObject.FindWithTag("Player"))
        {
            GameObject go = Instantiate(PlayerPrefab);
        
            this.player = go.transform;
            Object.DontDestroyOnLoad(player.gameObject);
            this.camera = this.player.transform.GetChild(1);
            characterInput = this.player.GetComponent<CharacterInput>();
        }
        else
        {
            GameObject go = GameObject.FindWithTag("Player");
            this.player = go.transform;
            Object.DontDestroyOnLoad(player.gameObject);
            this.camera = this.player.transform.GetChild(1);
            characterInput = this.player.GetComponent<CharacterInput>();
        }
        
        this.player.position = new Vector3(0, 30f, 0);

        if (e_PlayerHurt == null)
        {
            e_PlayerHurt = new UnityEvent();
        }
        
        if (Seed != 0)
        {
            Random.InitState(Seed);    
        }
        else
        {
            int s = Random.Range(0, int.MaxValue);
            Random.InitState(s);
            Debug.Log(s);
        }
        
    }

    public GenerationPreset GetRandomGeneration()
    {
        return LstGenerationPreset[Random.Range(0, LstGenerationPreset.Count)];
    }
    

    public void PlayerHurt(int dommage)
    {
        this.playerLife -= dommage;
        e_PlayerHurt.Invoke();
    }

}
