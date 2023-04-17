using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    public GameObject EnnemiePrefab;

    private GameManager GM;
    // Start is called before the first frame update
    void Start()
    {
        GM = GameManager.instance;
    
    }

    // Update is called once per frame
    void Update()
    {
        
        GameObject go = Instantiate(EnnemiePrefab, this.transform.position, Quaternion.identity);
        go.transform.SetParent(this.transform);
        go.transform.position = Random.insideUnitCircle * GM.CylenderRadius; // faire un vecteur 2 pas un 3 
    }
}
