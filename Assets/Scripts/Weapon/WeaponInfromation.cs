using UnityEngine;

public class WeaponInfromation : MonoBehaviour
{
    /// <summary>
    /// Ce script permet de stocker des var utiles des armes pour le WeaponManager.
    /// </summary>

    [Header("Values")] 
    public float reach;
    public LayerMask ennemiesLayer;
    [Header("Conpenents")]
    public Rigidbody Rigidbody;
    public Collider Collider;
    public I_WeaponInterface WeaponInterface;
    public Animator animator;
    
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
        
        if (animator != null)
        {
            animator.enabled = false;
        }
    }

    public void HandSetting(Transform tr)
    {
        if (animator != null)
        {
            animator.enabled = true;
        }
        
        this.transform.rotation = Quaternion.LookRotation(tr.forward);
        this.transform.SetParent(tr);
        this.transform.localPosition = Vector3.zero;
        this.Rigidbody.isKinematic = true;
        this.Collider.enabled = false;
        this.transform.localScale = Vector3.one;
        
    }

    // setting fonction quand une arme est drop
    public void DropSetting()
    {
        if (animator != null)
        {
            animator.enabled = false;
        }
        
        this.transform.parent = null;
        this.Rigidbody.isKinematic = false;
        this.Collider.enabled = true;
        this.transform.localScale = Vector3.one;
    }

    void Update()
    {
        
    }
}
