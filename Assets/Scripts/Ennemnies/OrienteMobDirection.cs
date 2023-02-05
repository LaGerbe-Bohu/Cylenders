using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrienteMobDirection : MonoBehaviour
{

    public MobInput mp;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       this.transform.rotation =  Quaternion.Lerp(this.transform.rotation , Quaternion.LookRotation(new Vector3(mp.getDirection().x,0,mp.getDirection().y),this.transform.up),Time.deltaTime*5f);
    }
}
