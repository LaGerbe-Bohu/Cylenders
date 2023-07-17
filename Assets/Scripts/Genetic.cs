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


    private GameObject First;

    public void IntializeGenetic()
    {
        
        //generation de la population
        population = new Person[nbIndiv];
      
        
        GameObject go = Instantiate(CreaturePrefab, Vector3.zero, Quaternion.identity);
        go.transform.SetParent(this.transform);
     
        
        population[0] = new Person();
        population[0].CD = go.GetComponent<CreatureDeplacement>();
        population[0].CD.CG.Generator(population[0].CD.CG.seed);
        population[0].CD.Initialize(this,0);
        population[0].nn = population[0].CD.nn;
        population[0].score = float.MaxValue;
        population[0].CD = go.GetComponent<CreatureDeplacement>();
        First = Instantiate(go);
        First.name = "FIRST";
        
        
        for (int idx = 1; idx < nbIndiv; idx++)
        {
            go = Instantiate(go);
            go.transform.SetParent(this.transform);
            population[idx] = new Person();
            population[idx].CD = go.GetComponent<CreatureDeplacement>();
            population[idx].CD.CG = go.GetComponent<CreaturesGenerator>();
            population[idx].CD.CG.firstLimb = population[0].CD.CG.firstLimb;
            population[idx].CD.Initialize(this,idx);
            population[idx].nn = population[idx].CD.nn;
            population[idx].score = float.MaxValue;
            population[idx].CD = go.GetComponent<CreatureDeplacement>();
           
        }

        best = population[0];
        best.score = float.MaxValue;
        // start la coroutine
        StartCoroutine(ProcessGenetic());

    }

    public void Respawn(int i)
    {
        Transform go = population[i].CD.transform;

        for (int j = 0; j < go.transform.childCount; j++)
        {
            go.GetChild(j).transform.position = First.transform.GetChild(j).transform.position;
            go.GetChild(j).transform.localScale = First.transform.GetChild(j).transform.localScale;
            go.GetChild(j).transform.rotation = First.transform.GetChild(j).transform.rotation;
        }
        
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
            population[i].CD.prewarm(this,i);
            StartCoroutine( population[i].CD.fitness(this));
        }
        
    }
    
    private Person croisement(Person a,Person b,Person d)
    {

        List<List<float>> bytesA = a.nn.wi;
        List<List<float>> bytesB = b.nn.wi;

        List<List<float>> finalbyte = new List<List<float>>();
   
        finalbyte = new List<List<float>>(bytesB);
        
        for (int i = 0; i < Decimal.Floor(bytesB.Count/2); i++)
        {
            for (int j = 0; j < bytesB[0].Count; j++)
            {
                finalbyte[i][j] =  bytesA[i][j];
            }
        }
        
        d.nn.wi = new List<List<float>>(finalbyte);
        
        bytesA = a.nn.wo;
        bytesB = b.nn.wo;

        finalbyte = new List<List<float>>();

        finalbyte = new List<List<float>>(bytesB);
        
        for (int i = 0; i < Decimal.Floor(bytesB.Count/2); i++)
        {
            for (int j = 0; j <bytesB[0].Count; j++)
            {
                finalbyte[i][j] =  bytesA[i][j];
            }
        }
     
        d.nn.wo = new List<List<float>>(finalbyte);
        
        if (Random.Range(1, 100) <= mutation)
        {
            
           
            bool wo = false;
           
            finalbyte = new List<List<float>>(d.nn.wi);
            if (Random.Range(0f, 1f) < .5f)
            {
                 wo = true;
                 finalbyte = new List<List<float>>(d.nn.wo);
            }
            
            
            float r = Random.Range(0, finalbyte.Count); 
            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < finalbyte[0].Count; j++)
                {
                    finalbyte[i][j] = Random.Range(-2.0f, 2.0f);
                }
            }


            if (wo)
            {
                d.nn.wo = new List<List<float>>(finalbyte);
            }
            else
            {
                d.nn.wi = new List<List<float>>(finalbyte);
            }
          
        }
        
        
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
