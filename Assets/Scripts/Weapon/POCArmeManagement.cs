using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POCArmeManagement : MonoBehaviour
{


    public GameObject Bullet;

    private bool fire = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && !fire)
        {
            GameObject go = Instantiate(Bullet, this.transform.position, Quaternion.identity);
            go.transform.SetParent(this.transform);
            Rigidbody rb =  go.GetComponent<Rigidbody>();
            
            rb.AddForce(Camera.main.transform.forward*250f,ForceMode.Impulse);
            fire = true;

        }

        if (Input.GetMouseButtonUp(0))
        {
            fire = false;
        }
        
        
        
        
    }
}
