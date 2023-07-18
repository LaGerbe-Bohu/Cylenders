using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using UnityEngine;

public class GeneticTest : MonoBehaviour
{
    public GameObject prefab;
    private Person[] population;

    public GeneticCube gen;
    // Start is called before the first frame update
    void Start()
    {

    
        population = new Person[3];
        int idx = 0;
        for (int j = 0; j < 3; j++)
        {
            GameObject go = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            go.transform.position = this.transform.position;
            go.transform.SetParent(this.transform);
            population[idx] = new Person();
            population[idx].BC = go.GetComponent<BasicCreature>();
            population[idx].BC.Initialization();
            population[idx].nn = population[idx].BC.nn;
            population[idx].score = float.MaxValue;
            idx++;
        }
        
        
        StartCoroutine(testCross());

    }

    IEnumerator testCross()
    {
        for (int i = 0; i < 2; i++)
        {
            population[i].BC.prewarm();
           yield return StartCoroutine( population[i].BC.fitness(new GeneticCube()));
           
           DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(saveNN));
           
           saveNN savebest = new saveNN()
           {
               wi =population[i].nn.wi,
               wo = population[i].nn.wo
           };
           MemoryStream msObj = new MemoryStream();
           js.WriteObject(msObj, savebest);
           msObj.Position = 0;
           StreamReader sr = new StreamReader(msObj);
           string json = sr.ReadToEnd();
           sr.Close();
           msObj.Close();
            
            
           File.WriteAllText(Application.dataPath+"/base+"+i+".json", json);

             
           
        }

        population[2] = gen.croisement(population[0], population[1], population[2]);
        population[2].BC.prewarm();
        yield return StartCoroutine(population[2].BC.fitness(new GeneticCube()));
        
        
        for (int i = 0; i < 3; i++)
        {
            saveNN savebest = new saveNN()
            {
                wi =population[i].nn.wi,
                wo = population[i].nn.wo
            };
          
            
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(saveNN));
            MemoryStream msObj = new MemoryStream();
            js.WriteObject(msObj, savebest);
            msObj.Position = 0;
            StreamReader sr = new StreamReader(msObj);
            string json = sr.ReadToEnd();
            sr.Close();
            msObj.Close();
            
            
            File.WriteAllText(Application.dataPath+"/best_+"+i+".json", json);

        }
        
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
