using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(GeneticCube))]
[CanEditMultipleObjects]
public class GeneticEditor : Editor
{
    public override void OnInspectorGUI() {
        
        DrawDefaultInspector();
        
        if (GUILayout.Button("SaveBest"))
        {
           (target as GeneticCube).SaveBest();
        }
        
          
        if (GUILayout.Button("LoadBest"))
        {
            (target as GeneticCube).loadBest();
        }
        
        
    }
}
