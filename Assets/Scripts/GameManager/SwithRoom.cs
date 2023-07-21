using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwithRoom : MonoBehaviour
{
    private bool swithing = false;
    private CharacterController CC;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.instance.mobSpawner.EnnemieCounter() <= 0 && !swithing && CC.IsGrounded())
        {
            GameManager.instance.InAnimiantion = true;
            swithing = true;
            StartCoroutine(change());
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.instance.mobSpawner.EnnemieCounter() <= 0 && !swithing && CC.IsGrounded())
        {
            GameManager.instance.InAnimiantion = true;
            swithing = true;
            StartCoroutine(change());
        }
    }
    

    IEnumerator change()
    {
        yield return new WaitForSeconds(2.3f);
        SceneManager.LoadScene(0);
    
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.player.transform.position = this.transform.position;
        CC = GameManager.instance.player.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
