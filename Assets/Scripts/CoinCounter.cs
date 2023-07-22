using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CoinCounter : MonoBehaviour
{
    private TextMeshProUGUI TMP;
    private GameManager gm;
    
    // Start is called before the first frame update
    void Start()
    {
        TMP = GetComponent<TextMeshProUGUI>();
        gm = GameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        TMP.text = gm.PI.Coins.ToString();
    }
}
