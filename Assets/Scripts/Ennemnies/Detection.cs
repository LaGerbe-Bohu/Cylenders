using UnityEngine;

public class Detection : MonoBehaviour
{
   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Bullet"))
      {
         Destroy(this.gameObject);
      }
   }
}
