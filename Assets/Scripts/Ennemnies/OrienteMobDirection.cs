using UnityEngine;

public class OrienteMobDirection : MonoBehaviour
{

    public MobInput mp;
    

    // Update is called once per frame
    void Update()
    {
        if (mp.getDirection() == Vector2.zero) return;
       this.transform.rotation =  Quaternion.Lerp(this.transform.rotation , Quaternion.LookRotation(new Vector3(mp.getDirection().x,0,mp.getDirection().y),this.transform.up),Time.deltaTime*5f);
    }
}
