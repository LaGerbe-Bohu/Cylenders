using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CoinCollector : MonoBehaviour
{
    private GameManager GM;

    private Rigidbody RB;
    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        GM = GameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
     
        Vector3 target = GM.cameraPlayer.transform.position + Vector3.down * 1.5f;
        if (Vector3.Distance(this.transform.position,target ) < 5)
        {
        
            this.transform.position = Vector3.MoveTowards(this.transform.position,
                target, Time.deltaTime*20.0f);
        }
        
        if (Vector3.Distance(this.transform.position,target ) < .5)
        {
            GM.PI.Coins++;
            Destroy(this.gameObject);
        }
    }
}
