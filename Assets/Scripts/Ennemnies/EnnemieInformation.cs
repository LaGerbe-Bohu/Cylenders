using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class EnnemieInformation : MonoBehaviour
{
    public float life = 3f;
    public List<Renderer> lstRender;
    public Rigidbody RB;
    public MobInput MI;
    public enablerHitEnnemie enablerHit;
    public GameObject prefab;
    public Transform footPosition;
    public void Hurt(Vector3 source)
    {
        enablerHit.Hit();
    }
    
    private void OnDrawGizmosSelected()
    {
        if (footPosition == null)
            return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(footPosition.position,0.1f);
    }
    
}
