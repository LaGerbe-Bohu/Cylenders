using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GenerationPreset", menuName = "BOHU/GenerationPreset", order = 1)]
public class GenerationPreset : ScriptableObject
{
    public Vector4 startPoint;
    public float treshold;
    public Color color;
}
