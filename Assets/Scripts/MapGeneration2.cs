using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[System.Serializable]
public struct StructGenerationSettings
{
    public GameObject strcture;
    public float number;
}

public class MapGeneration2 : MonoBehaviour
{
    public int size;
    public Renderer rendererCompenent;
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;
    public StableDiffusionImage2Image im2im;
 
    public float yScale = 1.0f;


    private Texture2D texture;
    private GameManager GM;
    private Vector3 position;
    
    [Header("Values")] 
    public Vector4 startPoint;
    public float treshold;
    public Color color;
    public List<StructGenerationSettings> lstStructures;

    [HideInInspector]
    public List<StructuresManager> str;
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

    public void Start()
    {
        str = new List<StructuresManager>();
    }

    public IEnumerator GenerateIa()
    {
        im2im.Generate();
        while(im2im.outputTexture == null)
        {
            yield return null;
        }
        texture = im2im.outputTexture;

        rendererCompenent.material.color = color;
        position = this.transform.position;


        Vector3[] vertices;
        Mesh m = meshFilter.mesh;
        vertices = m.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2Int texCoord = new Vector2Int((int)(m.uv[i].x * texture.width), (int)(m.uv[i].y * texture.height));
            texCoord = new Vector2Int((texCoord.x - 1) * 2, (texCoord.y - 1) * 2);
            Color v = texture.GetPixel(texCoord.x, texCoord.y);
            float height = ((1 - v.r) - 0.3f);

            vertices[i] += Vector3.forward * ((height) / 100f) * yScale;
        }

        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.RecalculateBounds();
        meshCollider.sharedMesh = m;

        position.z += 0.04f;
        this.transform.position = position;
    }

    public void loadStruct()
    {
        for (int i = 0; i < lstStructures.Count; i++)
        {
            for (int j = 0; j < lstStructures[i].number; j++)
            {

                GameObject go = Instantiate(lstStructures[i].strcture, this.transform, true);
                go.transform.Rotate(Vector3.up, Random.Range(-180f, 180f));
                go.SetActive(false);
                go.hideFlags = HideFlags.HideInHierarchy;
                StructuresManager t = go.GetComponent<StructuresManager>();
                bool find = t.findPlace(str);

                if (go != null && find)
                {
                    go.hideFlags = HideFlags.None;
                    structe++;
                    str.Add(t);
                    go.SetActive(true);
                }
                else if (go != null)
                {
                    Destroy(go);
                }
            }

        }
    }

}
