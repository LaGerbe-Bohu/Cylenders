using System.Collections;
using UnityEngine;

public class selfDestruction : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(counter());
    }

    IEnumerator counter()
    {
        yield return new WaitForSeconds(3f);
        
        Destroy(this.gameObject);
    }
    
}
