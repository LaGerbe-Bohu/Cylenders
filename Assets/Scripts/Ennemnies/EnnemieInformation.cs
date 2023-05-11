using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class EnnemieInformation : MonoBehaviour
{
    public float life = 3f;
    public List<Renderer> lstRender;
    public Rigidbody RB;
    public MobInput MI;
    public enablerHitEnnemie enablerHit;
    public void Hurt(Vector3 source)
    {
        Vector3 direction = Vector3.ProjectOnPlane( (this.transform.position- source),Vector3.up).normalized;
        RB.AddForce(direction*50f,ForceMode.VelocityChange);
        MI.setDirection(Vector2.zero);
        enablerHit.Hit();
    }
    
}
