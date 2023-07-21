using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public struct Loot
{
    [Range(0.0f,1.0f)]
    public float rate;
    public GameObject prefab;
}


public class LootManager : MonoBehaviour
{
    public void Drop(List<Loot> table,Vector3 position,int nb)
    {
        for (int i = 0; i < table.Count; i++)
        {
            if (Random.Range(0f, 1f) <= table[i].rate)
            {
                GameObject go = Instantiate(table[i].prefab, position, Quaternion.identity);
                Vector3 offset = Random.insideUnitSphere*2.0f;
                go.transform.position += new Vector3(offset.x, Mathf.Abs(offset.y), offset.z);
                go.transform.SetParent(this.transform);    
            }
        }
    }
    
}
