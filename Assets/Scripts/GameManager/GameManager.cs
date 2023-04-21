using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton qui me permettra d'accéder à certiaines variables n'importe d'où dans le code
    /// </summary>
    /// 
    public static GameManager instance;
    public int playerLife = 5;
    [SerializeField] private Renderer cylenderRender;
    
    //public field
    [FormerlySerializedAs("Player")] public Transform player;
    public Transform camera;
    
    [HideInInspector]
    public float CylenderRadius;
    private void Awake()
    {
        instance = this;
        CylenderRadius = cylenderRender.bounds.extents.magnitude/2f;
        
    }

}
