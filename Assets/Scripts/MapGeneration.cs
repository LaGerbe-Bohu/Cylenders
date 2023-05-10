using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public struct StructGenerationSettings
{
    public GameObject strcture;
    public float number;
}

public class MapGeneration : MonoBehaviour
{
    public int size;
    public Renderer rendererCompenent;
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;
    private Texture2D texture;
    private GameManager GM;
    private GenerationPreset Preset;
    
    [Header("Values")] 
    private Vector4 startPoint;
    private float treshold;
    private Color color;
    private List<StructGenerationSettings> lstStructures;


    private Texture2D GenerateDC(Texture2D texture)
    {
        
        int h = texture.width;

        for (int x = 0; x < h; x++)
        {   
            for (int y = 0; y < h; y++)
            {
                texture.SetPixel(x,y,Color.black);
            }
        }
        
        Func<float> _ = () => (Random.Range(0f,1f));

        float v = startPoint.x;
        texture.SetPixel(0,0,new Color(v,v,v,1));
        v = startPoint.y;
        
        texture.SetPixel(0,h-1,new Color(v,v,v,1));
        v = startPoint.z;
        texture.SetPixel(h-1,h-1,new Color(v,v,v,1));
        v = startPoint.w;
        texture.SetPixel(h-1,0,new Color(v,v,v,1));

        int i = h - 1;
        int id = 0;
        while (i > 1)
        {
            id = i / 2;

            float sum = 0;
            for (int x = id; x < h; x+=i)
            {
                for (int y = id; y < h; y+=i)
                {
                    sum = ( texture.GetPixel(x - id, y - id).r + texture.GetPixel(x - id, y + id).r + texture.GetPixel(x + id,y + id).r + texture.GetPixel(x + id,y - id).r )/(float)4f ;
                    texture.SetPixel(x,y,new Color(sum,sum,sum,1));
                }
            }

            int décalage = 0;

            for (int x = 0; x < h; x += id)
            {
                if (décalage == 0)
                {
                    décalage = id;
                }
                else
                {
                    décalage = 0;
                }
                
                for (int y = décalage; y < h; y += i)
                {
                    float somme = 0;
                    int n = 0;

                    if (x >= id)
                    {
                        somme += texture.GetPixel(x - id, y).r;
                        n++;
                    }

                    if (x + id < h)
                    {
                        somme += texture.GetPixel(x + id, y).r;
                        n++;
                    }

                    if (y >= id)
                    {
                        somme += texture.GetPixel(x, y - id).r;
                        n++;
                    }

                    if (y + id < h)
                    {
                        somme += texture.GetPixel(x, y + id).r;
                        n++;
                    }

                    float s = (somme / (float)n) + Random.Range((float)-id/(float)treshold,(float)id/(float)treshold);
                    texture.SetPixel(x,y,new Color(s,s,s,1));
                }   
            }

            i = id;
        }
        
        texture.Apply();
        return texture;
    }

    private int structe = 0;
    // Start is called before the first frame update
    void Start()
    {
        Preset = GameManager.instance.GetRandomGeneration();
        this.color = Preset.color;
        this.startPoint = Preset.startPoint;
        this.treshold = Preset.treshold;
        this.lstStructures = Preset.lstStructures;
        
        texture = new Texture2D(size, size);
        texture = GenerateDC(texture);
        rendererCompenent.material.color = color;
        
        
        Vector3[] vertices;
        Mesh m = meshFilter.mesh;
        vertices = m.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2Int texCoord = new Vector2Int((int) (m.uv[i].x * texture.width), (int) (m.uv[i].y * texture.height));
            Color v = texture.GetPixel(texCoord.x, texCoord.y);
            float height = ((1-v.r)-0.3f);  
         
            vertices[i] += Vector3.forward * (height) / 100f;
        }

        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.RecalculateBounds();
        meshCollider.sharedMesh = m;


        List<StructuresManager> str = new List<StructuresManager>();
        for (int i = 0; i < lstStructures.Count; i++)
        {
            for (int j = 0; j < lstStructures[i].number; j++)
            {
                GameObject go =  Instantiate(lstStructures[i].strcture, this.transform, true);
                structe++;
                StructuresManager t =  go.GetComponent<StructuresManager>();
                t.findPlace(str);
            
                if (go != null)
                {
                    str.Add(t);
                }
            }

        }
        
    }
    
}
