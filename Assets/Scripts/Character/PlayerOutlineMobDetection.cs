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

    private EventDetection detectEvent;
    private Collider lastDetect;
    private EventDetection unDetectedEvent;
    // Start is called before the first frame update
    void Start()
    {
        GM = GameManager.instance;
        detectEvent = new EventDetection();
        unDetectedEvent = new EventDetection();
        detectEvent.AddListener(Detect);
        unDetectedEvent.AddListener(UnDetect);
        
    }

    
    private  void Detect(Collider coll){
        
 
        List<Renderer> lst =  coll.GetComponent<EnnemieInformation>().lstRender;
        for (int i = 0; i < lst.Count; i++)
        {
            
            Debug.Log("rentre");
            Material[] mat = lst[i].materials;
            
            Array.Resize(ref mat,mat.Length+1);
            mat[mat.Length - 1] = outlineMaterial;
            lst[i].materials = mat;
        }
    }
    
        
    private void UnDetect(Collider coll){
        
        List<Renderer> lst =  lastDetect.GetComponent<EnnemieInformation>().lstRender;
        
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
        if (Physics.Raycast(this.transform.position, GM.camera.transform.forward,out hit, GM.PlayerReach, LM))
        {
            if (lastDetect != null && lastDetect != hit.collider)
                unDetectedEvent.Invoke(lastDetect);
            
            lastDetect = hit.collider; 
            detectEvent.Invoke(hit.collider);
        }
        else
        {
            if(lastDetect != null)
                unDetectedEvent.Invoke(lastDetect);
        }
        
    }
}
