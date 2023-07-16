using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwithRoom : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.InAnimiantion = true;
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
