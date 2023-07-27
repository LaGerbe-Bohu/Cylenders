using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject cylindre;
    public GameObject Pilier;
    public GeneticCube gc;
    public int nbrStruct;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MapManagerC());
    }

    IEnumerator MapManagerC()
    {
        MapGeneration2 cyl = cylindre.GetComponent<MapGeneration2>();
        GameObject p = Instantiate(Pilier, Pilier.transform.position, Pilier.transform.rotation);
        MapGeneration2 pilier = p.transform.GetChild(0).GetComponent<MapGeneration2>();
        

        yield return StartCoroutine(pilier.GenerateIa());
        yield return StartCoroutine(cyl.GenerateIa());

        StructGenerationSettings s = new StructGenerationSettings();
        s.strcture = p;
        s.number = nbrStruct;
        p.SetActive(false);

        cyl.lstStructures.Add(s);
        cyl.loadStruct();
        gc.IntializeGenetic();

    }
   

}
