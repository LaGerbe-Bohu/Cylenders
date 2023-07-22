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
    
    public List<ShopItem> lstShopItems;
    private List<ShopItem> selectedItem;

    private GameManager GM;
    // Start is called before the first frame update
    void Start()
    {
        GM = GameManager.instance;
        selectedItem = new List<ShopItem>();
        for (int i = 0; i < 3; i++)
        {
            ShopItem item = lstShopItems[Random.Range(0, lstShopItems.Count)];
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
