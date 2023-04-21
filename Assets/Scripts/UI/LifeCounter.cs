using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LifeCounter : MonoBehaviour
{
    public GameObject prefabHeart;
    public Canvas Canevas;
    private RectTransform img;
    private GameManager GM;
    // Start is called before the first frame update
    void Start()
    {
      
        GM = GameManager.instance;
        GM.e_PlayerHurt.AddListener(UpdateHeartDisplay);
        img = prefabHeart.GetComponent<RectTransform>();
        
        UpdateHeartDisplay();
    }

    public void UpdateHeartDisplay()
    {
        foreach (Transform tr in this.transform)
        {
            Destroy(tr.gameObject);
        }

        for (int i = 0; i < GM.playerLife; i++)
        {
            GameObject GO = Instantiate(prefabHeart, this.transform);
            GO.transform.position =
                new Vector3( (Canevas.pixelRect.width/2f) -(img.sizeDelta.x* (img.localScale.x) * Canevas.scaleFactor )*GM.playerLife/2f + (i*(img.sizeDelta.x* (img.localScale.x+0.01f)*Canevas.scaleFactor)) , GO.transform.position.y, GO.transform.position.z); 
        }
        
        
    }
    
    
}
