using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class WeaponManager : MonoBehaviour
{
    /// <summary>
    /// Ce script va permettre de déctecter un armes, et de la prendre dans sa main
    /// </summary>
    // Start is called before the first frame update

    public Transform[] slotForWeapon; // ce sont les slots où les armes iront se mettre 
    public CanHit[] canHits;
    public Animator[] animators;
    private Transform player;
    private Transform camera;
    [FormerlySerializedAs("WeaponLayer")] public LayerMask weaponLayer;
                                             
    private WeaponInfromation[] currentWeapon;
    private bool rIsHolding;
    private bool lIsHolding;
    private bool rArm;
    private bool lArm;
    private bool lDrop;
    private bool rDrop;

    private void Start()
    {
        currentWeapon = new WeaponInfromation[2];
        camera = GameManager.instance.camera;
        player = GameManager.instance.player;
    }
    
    void Update()
    {
        // DOIT êTRE REMPLACER PAR LE NEW INPUT SYSTEME
        // DOIT gérer aussi plusieurs mains
        if (Input.GetKeyDown(KeyCode.A))
        {
            rArm = true;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            lArm = true;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            rDrop = true;
        }
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            lDrop = true;
        }

        
        Assert.AreEqual(animators.Length,2);
        
        if (Input.GetMouseButtonDown(0) && rIsHolding && currentWeapon[0] != null)
        {
            animators[0].SetTrigger("Attack");
            StartCoroutine( Attack(0));
          
        }
        
        if (Input.GetMouseButtonDown(1) && lIsHolding && currentWeapon[1] != null) 
        {
            animators[1].SetTrigger("Attack");
            StartCoroutine( Attack(1));
        }

    }

    IEnumerator Attack(int i)
    {
        while (!canHits[i].Hit)
        {
            yield return new WaitForEndOfFrame();
        }
        
        currentWeapon[i].WeaponInterface.WeaponAction();
    }
    
    
    void FixedUpdate()
    {
        RaycastHit hit;

        if (Physics.Raycast(player.transform.position, camera.transform.forward, out hit,GameManager.instance.PlayerReach,weaponLayer))
        {
            if (lArm && !lIsHolding)
            {
                currentWeapon[1] = hit.collider.GetComponent<WeaponInfromation>();
                currentWeapon[1].HandSetting((slotForWeapon[1]));
                lIsHolding = true;
            }
            
            if (rArm && !rIsHolding)
            {
                currentWeapon[0] =  hit.collider.GetComponent<WeaponInfromation>();
                currentWeapon[0].HandSetting(slotForWeapon[0]);
                rIsHolding = true;
            }
        }

        if (rIsHolding && rDrop)
        {
            rIsHolding = false;
            currentWeapon[0].DropSetting();
        }

        if (lIsHolding && lDrop)
        {
            lIsHolding = false;
            currentWeapon[1].DropSetting();
        }

        lDrop = false;
        rDrop = false;
        rArm = false;
        lArm = false;
    }
}
