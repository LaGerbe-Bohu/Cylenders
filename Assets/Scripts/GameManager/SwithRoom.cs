using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwithRoom : MonoBehaviour
{
    public bool next = false;
    private bool swithing = false;
    private CharacterController CC;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.instance.mobSpawner.EnnemieCounter() <= 0 && !swithing && CC.IsGrounded())
        {
            swithing = true;
            if (next)
            {
                GameManager.instance.StartGame();   
            }
            else
            {
                GameManager.instance.NextLevel();
            }
                
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.instance.mobSpawner.EnnemieCounter() <= 0 && !swithing && CC.IsGrounded())
        {
            swithing = true;
            
            if (next)
            {
                GameManager.instance.StartGame();   
            }
            else
            {
                GameManager.instance.NextLevel();
            }
            
        }
    }
    

  
    // Start is called before the first frame update
    void Start()
    {
        CC = GameManager.instance.player.GetComponent<CharacterController>();

        if (!next)
        {
            GameManager.instance.player.transform.position = this.transform.position;        
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
