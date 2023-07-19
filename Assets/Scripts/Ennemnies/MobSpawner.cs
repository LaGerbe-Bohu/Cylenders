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
    public void SpawnCharacter()
    {
        GM = GameManager.instance;
        
        while (counter < maxMob)
        {

            bool find = false;
            int gdf = 0;
            while (!find && gdf < 1000)
            {
                Vector3 randompos = Random.insideUnitCircle * GM.CylenderRadius;
                RaycastHit hit;
                if (Physics.Raycast(new Vector3(randompos.x, 100, randompos.y),Vector3.down,out hit,1000f,groundLayer) && !Physics.Raycast(new Vector3(randompos.x, 100, randompos.y),Vector3.down,1000f,negatifLayer))
                {
                    GameObject go = Instantiate(EnnemiePrefab.prefab, hit.point+new Vector3(0,-EnnemiePrefab.footPosition.localPosition.y,0), Quaternion.identity);
                    go.transform.SetParent(this.transform);
                    find = true;
                }
                gdf++;
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
