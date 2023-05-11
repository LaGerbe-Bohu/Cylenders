using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    public int maxMob;
    public GameObject EnnemiePrefab;
    public LayerMask groundLayer;
    private GameManager GM;

    private int counter = 0;
    // Start is called before the first frame update
    public void SpawnCharacter()
    {
        GM = GameManager.instance;
        
        while (counter < maxMob)
        {
            GameObject go = Instantiate(EnnemiePrefab, this.transform.position, Quaternion.identity);
            go.transform.SetParent(this.transform);
            Vector3 randompos = Random.insideUnitCircle * GM.CylenderRadius;
            RaycastHit hit;

            if (Physics.Raycast(new Vector3(randompos.x, 10, randompos.y),Vector3.down,out hit,1000f,groundLayer))
            {
                go.transform.position = new Vector3(randompos.x, hit.point.y, randompos.y);    
            }
            counter++;
        }

    }


}
