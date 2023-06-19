using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Json;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;


public class Person
{
    // le position n'est qu'un int parce que la liste de vertices est à une seule dimension
    public NN nn;
    public CreatureDeplacement CD;
    public float score;

}

public class Genetic : MonoBehaviour
{
    // nombre d'individu par génération
    public int nbIndiv = 10;
    public int mutation = 1;
    public float timeToSleep = 1f;
    public GameObject CreaturePrefab;
    public TextMeshProUGUI MPRO;
    
    
    private Person best;
    private Person[] population;

    private MapGeneration mapGeneration;
    private Transform[] lstSphere;
    
    
    [HideInInspector]
    public int nbCorotine;
    private NN bestNN;
    private void Start()
    {
        IntializeGenetic();
    }

    public void IntializeGenetic()
    {
        
        //generation de la population
        population = new Person[nbIndiv*nbIndiv];
        int idx = 0;
        for (int i =  -nbIndiv/2; i <  nbIndiv/2; i++)
            for (int j = -nbIndiv/2; j < nbIndiv/2; j++)
            {
                GameObject go = Instantiate(CreaturePrefab, Vector3.zero, Quaternion.identity);
                go.transform.position = this.transform.position + Vector3.right * i * 15 + Vector3.forward*j*15;
                go.transform.SetParent(this.transform);
                population[idx] = new Person();
                population[idx].CD = go.GetComponent<CreatureDeplacement>();
                population[idx].CD.Initialize();
                population[idx].nn = population[idx].CD.nn;
                population[idx].score = float.MaxValue;
                population[idx].CD = go.GetComponent<CreatureDeplacement>();
                idx++;
            }
        


        best = population[0];
        best.score = float.MaxValue;
        // start la coroutine
        StartCoroutine(ProcessGenetic());

    }

    IEnumerator ProcessGenetic()
    {
        int generation = 0;
        float bestscore = float.MaxValue;
        bestNN = population[0].nn;
        while (true)
        {
        
            Debug.Log("Start Fitness");

            nbCorotine = population.Length;

            
            calculFitness();

            while (nbCorotine > 0)
            {
                yield return null;
            }

            for (int i = 0; i < population.Length; i++)
            {
                population[i].score = population[i].CD.bestDistance;
            }
            
      
            population = Sort(population);
            
                  
            // voir si on n'a pas trouver un meilleur individu
            if (bestscore > population[^1].score)
            {
                bestNN = population[^1].nn;
                bestscore = population[^1].score;
            }

            
            // selection biaisé
            List<Person> perso = new List<Person>();
            for (int i = 0; i < population.Length; i++){
                int n = (int)(Mathf.InverseLerp(0, population.Length - 1, i) * 100)+1;
            
                for (int j = 0; j < n; ++j){

                    /*Create a new shared_ptr i number of times based on
                    the fitness. */
                    perso.Add(population[i]);
                }
            }


            MPRO.text = " generation : " + generation + " best : " + bestscore + " last " + population[^1].score;
            // reproduction
            for (int i = 0; i < nbIndiv; ++i){

                //initialisation
                int x = 0;
                int y = 0;

                //on cherche les parents, étant donné que la liste est biaisé il a plus de chance de tomber sur un individu fort
                x = Random.Range(0,perso.Count);
                y = Random.Range(0,perso.Count);

                //pour êtrecertain que les parents ne soient pas identique
                while ( perso[x] == perso[y] ){
                    y =  Random.Range(0,perso.Count);
                }
                
                //croisé
                population[i] = croisement(perso[x], perso[y],population[i]);
            }
            
            
            int k = Random.Range(0, population.Length);
            population[k].nn = new NN(bestNN);

            generation++;
            yield return new WaitForSeconds(0);
        }
    }

    
    public Person[] Sort(Person[] pop)
    {
        bool change = true;

        while (change)
        {
            change = false;
            for (int i = 0; i < pop.Length-1; i++)
            {
                if (pop[i].score < pop[i + 1].score)
                {
                    change = true;
                    (pop[i], pop[i + 1]) = (pop[i + 1], pop[i]);
                }
            }
        }

        return pop;
    }
    

    void calculFitness()
    {
        //calculFitness
        for (int i = 0; i < population.Length; i++)
        {
            population[i].CD.prewarm();
            StartCoroutine( population[i].CD.fitness(this));
        }
        
    }
    
    private Person croisement(Person a,Person b,Person d)
    {

        List<List<float>> bytesA = a.nn.wi;
        List<List<float>> bytesB = b.nn.wi;

        List<List<float>> finalbyte = new List<List<float>>();
        int r = Random.Range(0, bytesB.Count);
        int r2 = Random.Range(0, bytesB[0].Count);

        finalbyte = new List<List<float>>(bytesB);
        
        for (int i = 0; i < r; i++)
        {
            for (int j = 0; j < r2; j++)
            {
                finalbyte[i][j] =  bytesA[i][j];
            }
        }
     
        if (Random.Range(1, 100) <= mutation)
        {
            
            r = Random.Range(0, finalbyte.Count);
            r2 = Random.Range(0, finalbyte[0].Count);

            finalbyte[r][r2] = Random.Range(-2.0f, 2.0f);
       
        }
        
        d.nn.wi = finalbyte;
        d.score =  float.MaxValue;


        return d;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            saveNN savebest = new saveNN()
            {
                wi =bestNN.wi,
                wo = bestNN.wo
            };
            
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(saveNN));
            MemoryStream msObj = new MemoryStream();
            js.WriteObject(msObj, savebest);
            msObj.Position = 0;
            StreamReader sr = new StreamReader(msObj);
            string json = sr.ReadToEnd();
            sr.Close();
            msObj.Close();
            
            File.WriteAllText(Application.dataPath+"/best.json", json);

        }
    }
}
