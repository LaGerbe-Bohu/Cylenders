using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobFollowDirection : MonoBehaviour
{
    public Transform Render;
    public MobInput MI;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = new Vector3(MI.getDirection().x, 0, MI.getDirection().y);
        
        Quaternion lookQuaternion = Quaternion.LookRotation(forward);

        Render.transform.rotation = Quaternion.Lerp(Render.transform.rotation, lookQuaternion,Time.deltaTime
        *3.0f);
    }
    
    
}
