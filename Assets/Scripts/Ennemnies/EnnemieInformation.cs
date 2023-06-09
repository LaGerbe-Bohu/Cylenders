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
     
        enablerHit.Hit();
    }
    
}
