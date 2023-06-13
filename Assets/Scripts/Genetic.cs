using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Random = UnityEngine.Random;


class Person
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
    
    
    public void IntializeGenetic(Vector3[] _vert,MapGeneration MP)
    {
        
        //generation de la population
        mapGeneration = MP;
        population = new Person[nbIndiv];

        for (int i = 0; i < nbIndiv; i++)
        {
            GameObject go = Instantiate(CreaturePrefab,Vector3.zero, Quaternion.identity);
            go.SetActive(false);
            population[i] = new Person();
            population[i].CD = go.GetComponent<CreatureDeplacement>();
            population[i].nn = population[i].CD.nn;
            population[i].score = float.MaxValue;
        }

        
        best = new Person();

        // start la coroutine
        StartCoroutine(ProcessGenetic());

    }

    IEnumerator ProcessGenetic()
    {
        int generation = 0;
        while (true)
        {
        
            population = calculFitness(population);
            
            //Person best = SortPerson(population)[0];
            
            
            // voir si on n'a pas trouver un meilleur individu
            for (int i = 0; i < population.Length; i++)
            {
                if (best.score < population[i].score)
                {
                    best = population[i];
                }

            }
            
            population[Random.Range(0, population.Length)] = best;
            
            
            // selection biaisé
            List<Person> perso = new List<Person>();
            for (int i = 0; i < population.Length; i++){
                int n = (int)(population[i].score*100);
            
            
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
                population[i] = croisement(perso[x], perso[y]);
            }
            

            generation++;
            yield return new WaitForSeconds(timeToSleep);
        }
    }
    
    
    private Person[]  calculFitness(Person[] person)
    {
        for (int i = 0; i < person.Length; i++)
        {
            bool b = person[i].CD.fitness();
        }

        return person;
    }

    private Person croisement(Person a,Person b)
    {

        Person p = new Person();

        List<List<float>> bytesA = a.nn.wi;
        List<List<float>> bytesB = b.nn.wi;

        List<List<float>> finalbyte = new List<List<float>>();
        int r = Random.Range(0, finalbyte.Count);
        int r2 = Random.Range(0, finalbyte[0].Count);

        
        for (int i = 0; i < finalbyte.Count; i++)
        {
            for (int j = 0; j < finalbyte[0].Count; j++)
            {
                finalbyte[i][j] =  bytesB[i][j];
            }
        }

        
        for (int i = 0; i < r; i++)
        {
            for (int j = 0; j < r2; j++)
            {
                finalbyte[i][j] =  bytesA[i][j];
            }
        }

        p.nn.wi = finalbyte;
        p.score = 0;
        
        if (Random.Range(1, 100) <= mutation)
        {
            
            r = Random.Range(0, finalbyte.Count);
            r2 = Random.Range(0, finalbyte[0].Count);

            finalbyte[r][r2] = Random.Range(-2.0f, 2.0f);
       
        }

        return p;
    }

}
