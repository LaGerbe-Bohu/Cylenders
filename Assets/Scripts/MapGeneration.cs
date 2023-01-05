using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGeneration : MonoBehaviour
{
      
    public Renderer rendererCompenent;
    private Texture2D texture;
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

        float v = _();
        texture.SetPixel(0,0,new Color(v,v,v,1));
        v = _();
        
        texture.SetPixel(0,h-1,new Color(v,v,v,1));
        v = _();
        texture.SetPixel(h-1,h-1,new Color(v,v,v,1));
        v = _();
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
                    float a = texture.GetPixel(x - id, y - id).r;
                    float b = texture.GetPixel(x - id, y + id).r;
                    float c = texture.GetPixel(x + id, y + id).r;
                     float d =  texture.GetPixel(x + id,y - id).r ;
                    sum = ( texture.GetPixel(x - id, y - id).r + texture.GetPixel(x - id, y + id).r + texture.GetPixel(x + id,y + id).r + texture.GetPixel(x + id,y - id).r )/(float)4f ;
         
                    sum += Random.Range(0,(float)id/(float)h);
                    sum = Mathf.Clamp01(sum);
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

                    float s = (somme / (float)n) + Random.Range((float)-id/(float)h,(float)id/(float)h);
                    s = Mathf.Clamp01(s);
                    texture.SetPixel(x,y,new Color(s,s,s,1));
                }   
            }

            i = id;
        }
        
        texture.Apply();
        return texture;
    }
    
        

    // Start is called before the first frame update
    void Start()
    {
        texture = new Texture2D(1024+1, 1024+1);
        texture = GenerateDC(texture);
        rendererCompenent.material.mainTexture = texture;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
