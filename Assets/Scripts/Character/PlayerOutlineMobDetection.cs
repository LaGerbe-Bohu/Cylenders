using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class EventDetection : UnityEvent<Collider>
{
}
public class PlayerOutlineMobDetection : MonoBehaviour
{
    private GameManager GM;
    public LayerMask LM;
    public Material outlineMaterial;
    public WeaponManager WM;
    
    private EventDetection detectEvent;
    private List<Collider> lastDetect;
    private EventDetection unDetectedEvent;

    private List<float> reach;
    // Start is called before the first frame update
    void Start()
    {
        GM = GameManager.instance;
        detectEvent = new EventDetection();
        unDetectedEvent = new EventDetection();
        detectEvent.AddListener(Detect);
        unDetectedEvent.AddListener(UnDetect);
        lastDetect = new List<Collider>();
    }

    
    private  void Detect(Collider coll){
        
 
        List<Renderer> lst =  coll.GetComponent<EnnemieInformation>().lstRender;
        for (int i = 0; i < lst.Count; i++)
        {
            Material[] mat = lst[i].materials;
            
            Array.Resize(ref mat,mat.Length+1);
            mat[mat.Length - 1] = outlineMaterial;
            lst[i].materials = mat;
        }
    }
    
        
    private void UnDetect(Collider coll){
        
        List<Renderer> lst =  coll.GetComponent<EnnemieInformation>().lstRender;
        
        for (int i = 0; i < lst.Count; i++)
        {
            Material[] mat = lst[i].materials;
            if (mat.Length <= 1)
            {
                continue;
            }

            Material[] temp =new Material[mat.Length-1];
            Array.Copy(mat, temp,temp.Length);
            
            lst[i].materials = temp;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        
        for (int i = 0; i < lastDetect.Count; i++)
        {
            if (lastDetect[i] != null)
            {
                unDetectedEvent.Invoke(lastDetect[i]);    
            }
                        
        }

        lastDetect = new List<Collider>();
        
        var Ennemies = Physics.SphereCastAll(this.transform.position,WM.Aoe, GM.cameraPlayer.transform.forward, WM.Reach, LM);

        for (int i = 0; i < Ennemies.Length; i++)
        {
           
                lastDetect.Add(Ennemies[i].collider);;
                detectEvent.Invoke(Ennemies[i].collider);
            
        }


    }
}
