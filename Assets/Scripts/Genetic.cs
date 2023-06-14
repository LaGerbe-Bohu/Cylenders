using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
    
    private Person best;
    private Person[] population;

    private MapGeneration mapGeneration;
    private Transform[] lstSphere;
    
    [HideInInspector]
    public int nbCorotine;

    private void Start()
    {
        IntializeGenetic();
    }

    public void IntializeGenetic()
    {
        
        //generation de la population
        population = new Person[nbIndiv];

        for (int i = 0; i < nbIndiv; i++)
        {
            GameObject go = Instantiate(CreaturePrefab,Vector3.zero, Quaternion.identity);
            go.transform.position = this.transform.position + Vector3.right * i * 10;
            go.transform.SetParent(this.transform);
            population[i] = new Person();
            population[i].CD = go.GetComponent<CreatureDeplacement>();
            population[i].CD.Initialize();
            population[i].nn = population[i].CD.nn;
            population[i].score = float.MaxValue;
            population[i].CD = go.GetComponent<CreatureDeplacement>();
        }


        best = population[0];

        // start la coroutine
        StartCoroutine(ProcessGenetic());

    }

    IEnumerator ProcessGenetic()
    {
        int generation = 0;
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
            if (best.score > population[^1].score)
            {
                best = population[^1];
                best.score = population[^1].score;
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
            
            Debug.Log(" generation : "+ generation+ " best : "+best.score);
            
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
            population[k].nn.wi = (best.nn.wi);

            generation++;
            yield return new WaitForSeconds(timeToSleep);
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
            population[i].CD.Initialize();
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

}
