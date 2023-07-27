using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text.Json;
using Newtonsoft.Json;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Person
{
    // le position n'est qu'un int parce que la liste de vertices est à une seule dimension
    public NN nn;
    public float score;
    public BasicCreature BC;

    public Person(Person p)
    {
        this.nn = new NN(p.nn);
        this.score = p.score;
    }

    public Person()
    {

    }

}

public class GeneticCube : MonoBehaviour
{
    // nombre d'individu par génération
    public int nbIndiv = 10;
    public int mutation = 1;
    public GameObject CreaturePrefab;
    public TextMeshProUGUI MPRO;
    public Transform target;
    public int TimeSumulation;
    public bool start = false;
    public Material basicMat;
    public Material bestMat;
    
    
    private Person best;
    private Person[] population;

    private MapGeneration mapGeneration;
    private Transform[] lstSphere;
    private Queue<IEnumerator> QueFintess;
    private List<string> csvLiner;
    private StreamWriter writer;
    
    [HideInInspector]
    public int nbCorotine;
    private NN bestNN;
    private void Start()
    {
        if (start)
        {
            IntializeGenetic();    
        }
        
    }

    public void IntializeGenetic()
    {
        
        //generation de la population
        population = new Person[nbIndiv];
        int idx = 0;
        for (int i = 0;i < nbIndiv ; i++)
        {
            GameObject go = Instantiate(CreaturePrefab, this.transform.position, Quaternion.identity);
            go.transform.position = this.transform.position;
            go.transform.SetParent(this.transform);
            population[idx] = new Person();
            population[idx].BC = go.GetComponent<BasicCreature>();
            population[idx].BC.Initialization(this);
            population[idx].nn = population[idx].BC.nn;
            population[idx].score = float.MaxValue;
            idx++;
        }

        QueFintess = new Queue<IEnumerator>();
        best = population[0];
        best.score = float.MaxValue;
        
        csvLiner = new List<string>();

        csvLiner.Add("Generation;Best Score");
        
        // start la coroutine
        StartCoroutine(ProcessGenetic());
    }
    
    List<List<float>> bestWi ;
    List < List<float> > bestWo ;
    int bestIdx = 0;
    IEnumerator ProcessGenetic()
    {
        int generation = 0;
        int bestidx = 0;
        float bestscore = float.MaxValue;
        bestWi = new List<List<float>>();
        bestWo = new List<List<float>>();
       
        while (true)
        {

            
            Debug.Log("Start Fitness");
        
            population[bestidx].BC.GetComponent<Renderer>().material = bestMat;

            nbCorotine = population.Length;
            calculFitness();
            
            while (nbCorotine > 0)
            {
                yield return null;
            }

            for (int i = 0; i < population.Length; i++)
            {
                population[i].score = population[i].BC.bestDistance;
                population[i].BC.GetComponent<Renderer>().material = basicMat;
            }
         
            
            population = Sort(population);
            
            // voir si on n'a pas trouver un meilleur individu
            if (bestscore > population[^1].score)
            {

                DeepCopy(population[^1].nn.wi, ref bestWi);
                DeepCopy(population[^1].nn.wo, ref bestWo);
                
                
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


            Debug.Log(" generation : " + generation + " best : " + bestscore + " lest generation :" +
                      population[^1].score);
            MPRO.text = " generation : " + generation + " best : " + bestscore;
            csvLiner.Add(generation.ToString() +" ; "+(bestscore).ToString());
            
            // reproduction
            for (int i = 0; i < population.Length; ++i){

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
 
            DeepCopy(bestWi, ref population[k].nn.wi);
            DeepCopy(bestWo, ref population[k].nn.wo);
            bestidx = k;
            
            generation++;
            yield return new WaitForFixedUpdate();
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
            population[i].BC.prewarm(this);
            StartCoroutine( population[i].BC.fitness(this));
        }
        
        
    }

    private void DeepCopy(List<List<float>> source,ref List<List<float>> dest)
    {
        dest = new List<List<float>>();
        for (int i = 0; i < source.Count; i++)
        {
            dest.Add(new List<float>(source[i]));
        }

    }
    
    public Person croisement(Person a,Person b,Person d)
    {

      
        Random.InitState((int)DateTime. Now. Ticks);
        List<List<float>> bytesA = new List<List<float>>();
        DeepCopy(a.nn.wi,ref bytesA);
        
        List<List<float>> bytesB = new List<List<float>>();
        DeepCopy(b.nn.wi,ref bytesB);
       

        List<List<float>> finalbyte = new List<List<float>>();
        DeepCopy(bytesB,ref finalbyte);
        
        /*
        
        for (int i = 0; i <finalbyte.Count; i++)
        {
            for (int j = 0; j < finalbyte[0].Count; j+=2)
            {
                finalbyte[i][j] =  bytesA[i][j];
            }
        }
        
        */
        
        
        for (int i = 0; i <finalbyte.Count; i++)
        {
            for (int j = 0; j < finalbyte[0].Count; j++)
            {
                finalbyte[i][j] = (bytesA[i][j] + bytesB[i][j])/2.0f;
            }
        }
        
        d.nn.wi = new List<List<float>>(finalbyte);
        DeepCopy(finalbyte,ref d.nn.wi);
        
        DeepCopy(a.nn.wo,ref bytesA);
        DeepCopy(b.nn.wo,ref bytesB);
        
        DeepCopy(bytesB,ref finalbyte);
        
        /*
      
        
        for (int i = 0; i < finalbyte.Count; i++)
        {
            for (int j = 0; j <finalbyte[0].Count; j+=2)
            {
                finalbyte[i][j] =  bytesA[i][j];
            }
        }
           */
        
        for (int i = 0; i < finalbyte.Count; i++)
        {
            for (int j = 0; j <finalbyte[0].Count; j+=2)
            {
                finalbyte[i][j] = (bytesA[i][j] + bytesB[i][j])/2.0f;
            }
        }
        
        d.nn.wo = new List<List<float>>(finalbyte);
        DeepCopy(finalbyte,ref d.nn.wo);
        
        
        if (Random.Range(1, 100) <= mutation)
        {

           
                bool wo = false;
           
                DeepCopy( d.nn.wi,ref finalbyte);
                if (Random.Range(0f, 1f) < .5f)
                {
                    wo = true;
                    finalbyte = new List<List<float>>(d.nn.wo);
                    DeepCopy( d.nn.wo,ref finalbyte);
                }   
            
            
                int bX = Random.Range(0, finalbyte.Count); 
                int eX = Random.Range(bX, finalbyte.Count); 
                for (int i = bX; i < eX; i++)
                {
                    for (int j = 0; j < finalbyte[0].Count; j++)
                    {
                        finalbyte[i][j] = Random.Range(-2.0f, 2.0f);
                    }
                }


                if (wo)
                {
                    DeepCopy(finalbyte,ref d.nn.wo);
                }
                else
                {
                    DeepCopy(finalbyte,ref d.nn.wi);
                }

        }
        
        
        d.score =  float.MaxValue;


        return d;
    }


    public void SaveBest()
    {
        saveNN savebest = new saveNN()
        {
            wi =bestWi,
            wo = bestWo
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

    public void loadBest()
    {
        string fileName = Application.dataPath+"/best.json";
        string jsonString = File.ReadAllText(fileName);
        saveNN saveNN = Newtonsoft.Json.JsonConvert.DeserializeObject<saveNN>(jsonString);
        
        GameObject go = Instantiate(CreaturePrefab, this.transform.position, Quaternion.identity);
        go.transform.position = this.transform.position;
        go.transform.SetParent(this.transform);
        go.GetComponent<BasicCreature>().Initialization(saveNN,this.target.position,this.transform.position,this.TimeSumulation);
        
    }
    
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveBest();
        }
        
           
        if (Input.GetKeyDown(KeyCode.F))
        {
            string filePath = Application.dataPath+"/GenerationSheet.csv";
            writer = new StreamWriter(filePath);
            
            

            for (int i = 0; i <csvLiner.Count; ++i)
            {
                writer.WriteLine(csvLiner[i]);
            }
            writer.Close();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < population.Length; i++)
            {
                if (bestIdx != i)
                {
                    population[i].BC.GetComponent<Fish>().Rendering();      
                }
              
                
            }
        }

    }
}
