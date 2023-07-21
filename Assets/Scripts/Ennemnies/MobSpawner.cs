using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MobSpawner : MonoBehaviour
{
    public int maxMob;
    public EnnemieInformation EnnemiePrefab;
    public LayerMask groundLayer;
    public LayerMask negatifLayer;
    private GameManager GM;

    private int counter = 0;
    // Start is called before the first frame update
    public void SpawnCharacter(List<StructuresManager> str)
    {
        GM = GameManager.instance;
        
        while (counter < maxMob)
        {

            bool find = false;
            int gdf = 0;
            while (!find && gdf < 1000)
            {
                gdf++;
                Vector3 randompos = Random.insideUnitCircle * GM.CylenderRadius;
                RaycastHit hit;
                
                if (Physics.Raycast(new Vector3(randompos.x, 100, randompos.y),Vector3.down,out hit,1000f,groundLayer))
                {
                    Vector3 position = hit.point + new Vector3(0, -EnnemiePrefab.footPosition.localPosition.y, 0);
                    bool near = false;
                    for (int i = 0; i < str.Count; i++)
                    {
                        
                        if (Vector3.Distance( Vector3.ProjectOnPlane(str[i].transform.position,Vector2.up) , Vector3.ProjectOnPlane(position,Vector2.up) ) < str[i].DistanceBounding)
                        {
                            near = true;
                            break;
                        }
                    }

                    if (near)
                    {
                        continue;           
                    }
                      
                    GameObject go = Instantiate(EnnemiePrefab.prefab, position , Quaternion.identity);
                    go.transform.SetParent(this.transform);
                    find = true;
                }
               
            }

            if (gdf >= 1000)
            {
                Debug.LogError("gdf supérieur à 1000");    
            }
         
            counter++;
        }
    }

    public int EnnemieCounter()
    {
        return this.transform.childCount;
    }
    
    public void Update()
    {
       
    }
}
