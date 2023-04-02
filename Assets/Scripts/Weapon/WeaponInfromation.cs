using Unity.VisualScripting;
using UnityEngine;

public class WeaponInfromation : MonoBehaviour
{
    /// <summary>
    /// Ce script permet de stocker des var utiles des armes pour le WeaponManager.
    /// </summary>

    public Rigidbody Rigidbody;
    public Collider Collider;
    
    // Start is called before the first frame update
    void Start()
    {
        if (Rigidbody == false)
        {
            Rigidbody = this.GetComponent<Rigidbody>();
        }

        if (Collider == false)
        {
            Collider = this.GetComponent<Collider>();
        }
    }

    public void HandSetting(Transform tr)
    {

        this.transform.rotation = Quaternion.LookRotation(tr.forward);
        this.transform.SetParent(tr);
        this.transform.position = tr.transform.position;
        this.Rigidbody.isKinematic = true;
        this.Collider.enabled = false;
        
    }

    public void DropSetting()
    {
        
        this.transform.parent = null;
        this.Rigidbody.isKinematic = false;
        this.Collider.enabled = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
