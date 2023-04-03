using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton qui me permettra d'accéder à certiaines variables n'importe d'où dans le code
    /// </summary>
    /// 
    public static GameManager instance;
    
    
    //public field
    [SerializeField]
    private Transform Player;
    
    public Transform getPlayer()
    {
        return this.Player;
    }

    private void Awake()
    {
        instance = this;
    }

}
