using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ShopItem
{
    public int cost;
    public GameObject prefab;
}
public class ShopManager : MonoBehaviour
{
    public Transform WeaponPosition;
    
    public List<ShopItem> lstShopItemslv1;
    public List<ShopItem> lstShopItemslv2;
    public List<ShopItem> lstShopItemslv3;
    private List<ShopItem> selectedItem;

    private GameManager GM;

    
    
    // Start is called before the first frame update
    void Start()
    {
        GM = GameManager.instance;
        selectedItem = new List<ShopItem>();
        float t = Mathf.Clamp( GameManager.instance.PI.Room / 10.0f,0,1.0f);
        
        for (int i = 0; i < 3; i++)
        {
            bool find = false;
            ShopItem item = new ShopItem();

            while (!find)
            {
                float a = (1 - t) * (1 - t);
                float b = 2 * t * (1 - t);
                float c = (t * t);

                if (Random.Range(0, 1f) < a)
                {
                    find = true;
                    item = lstShopItemslv1[Random.Range(0, lstShopItemslv1.Count)];
                    break;
                }
                
                if (Random.Range(0, 1f) < b)
                {
                    find = true;
                    item = lstShopItemslv2[Random.Range(0, lstShopItemslv2.Count)];
                    break;
                }
                
                if (Random.Range(0, 1f) < c)
                {
                    find = true;
                    item = lstShopItemslv3[Random.Range(0, lstShopItemslv3.Count)];
                    break;
                }

        
                
            }
            
            ShopItem tmp;
            tmp.cost = item.cost;
            tmp.prefab =  Instantiate(item.prefab, WeaponPosition.transform.position + Vector3.right*1f*i,Quaternion.identity); 
            tmp.prefab.transform.SetParent(this.transform);
             
            if (GM.PI.Coins < tmp.cost)
            {
                tmp.prefab.layer = LayerMask.NameToLayer("Default");
            }
            selectedItem.Add(tmp);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.childCount < 4)
        {
            for (int i = 0; i < selectedItem.Count; i++)
            {
                if (selectedItem[i].prefab.transform.parent != this.transform)
                {
                    GM.PI.Coins -= selectedItem[i].cost;
                    break;
                }
            }
            
            Destroy(this.gameObject);
        }
    }
}
