using UnityEngine;

public class WeaponInfromation : MonoBehaviour
{
    /// <summary>
    /// Ce script permet de stocker des var utiles des armes pour le WeaponManager.
    /// </summary>

    public Rigidbody Rigidbody;
    public Collider Collider;
    public I_WeaponInterface WeaponInterface;
    void Start()
    {
        // Peut être changer ca parce que c'est pas très propre
        if (Rigidbody == false)
        {
            Rigidbody = this.GetComponent<Rigidbody>();
        }

        if (Collider == false)
        {
            Collider = this.GetComponent<Collider>();
        }

        if (WeaponInterface != null)
        {
            WeaponInterface = this.GetComponent<I_WeaponInterface>();
        }
    }

    public void HandSetting(Transform tr)
    {

        this.transform.rotation = Quaternion.LookRotation(tr.forward);
        this.transform.SetParent(tr);
        this.transform.localPosition = Vector3.zero;
        this.Rigidbody.isKinematic = true;
        this.Collider.enabled = false;
        
    }

    // setting fonction quand une arme est drop
    public void DropSetting()
    {
        
        this.transform.parent = null;
        this.Rigidbody.isKinematic = false;
        this.Collider.enabled = true;
        
    }

    void Update()
    {
        
    }
}
