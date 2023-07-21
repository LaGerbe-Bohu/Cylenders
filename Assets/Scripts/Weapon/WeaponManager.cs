using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class WeaponManager : MonoBehaviour
{
    /// <summary>
    /// Ce script va permettre de déctecter un armes, et de la prendre dans sa main
    /// </summary>
    // Start is called before the first frame update
    public Transform[] fingers;
    public Transform[] slotForWeapon; // ce sont les slots où les armes iront se mettre 
    public Animator[] animators;
    public Transform renderRHandObject;
    public Transform riggedRightArm;
    public Transform riggedlefttArm;
    public float timeHit;
    [FormerlySerializedAs("WeaponLayer")] public LayerMask weaponLayer;
    
    
    
    [HideInInspector] public float Reach;
    [HideInInspector] public float Aoe;
    
    private WeaponInfromation[] currentWeapon;
    private Transform player;
    private Transform cameraPlayer;
    private bool rIsHolding;
    private bool lIsHolding;
    private bool rArm;
    private bool lArm;
    private bool lDrop;
    private bool rDrop;
    private bool[] coolDownAttack;
    
    
    private bool animateFingers = false;
    private Quaternion localRight;
    private Transform localLeft;
    public bool getAnimateFingers()
    {
        return this.animateFingers;
    }

    public Quaternion getlocalRight()
    {
        return this.localRight;
    }
    
    
    private void Start()
    {
        currentWeapon = new WeaponInfromation[2];
        cameraPlayer = GameManager.instance.cameraPlayer;
        player = GameManager.instance.player;

        if (fingers.Length > 0)
        {
            animateFingers = true;
        }

        Reach = 0;

        localRight = this.renderRHandObject.rotation;
        coolDownAttack = new bool[2]{false,false};
    }

    void Update()
    {
        // DOIT êTRE REMPLACER PAR LE NEW INPUT SYSTEME
        if (Input.GetKeyDown(KeyCode.E))
        {
            rArm = true;
        }

        if (Input.GetKeyDown(KeyCode.A))
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

        if (Input.GetMouseButtonDown(0) && rIsHolding && currentWeapon[0] != null && !coolDownAttack[0])
        {
            animators[0].SetTrigger("Attack");
            StartCoroutine( Attack(0));
   

        }
        
        if (Input.GetMouseButtonDown(1) && lIsHolding && currentWeapon[1] != null && !coolDownAttack[1]) 
        {
            animators[1].SetTrigger("Attack");
            StartCoroutine(Attack(1));
        }

    }
    

    IEnumerator Attack(int i)
    {
        coolDownAttack[i] = true;
        currentWeapon[i].WeaponInterface.WeaponAction();
     
        yield return new WaitForSeconds(currentWeapon[i].CoolDowwnTime);
      
        coolDownAttack[i] = false;
    }
    
    
    void FixedUpdate()
    {
        RaycastHit hit;

        if (Physics.Raycast(player.transform.position, cameraPlayer.transform.forward, out hit,GameManager.instance.PlayerReach,weaponLayer))
        {
            if (lArm && !lIsHolding)
            {
                currentWeapon[1] = hit.collider.GetComponent<WeaponInfromation>();

                Reach = MathF.Max(currentWeapon[1].Reach, currentWeapon[0] == null ? 0: currentWeapon[0].Reach);
                Aoe = MathF.Max(currentWeapon[1].AreaOfEffect, currentWeapon[0] == null ? 0: currentWeapon[0].AreaOfEffect);
                riggedlefttArm.gameObject.SetActive(false);
                currentWeapon[1].HandSetting((slotForWeapon[1]),this);
                lIsHolding = true;
            }
            
            if (rArm && !rIsHolding)
            {
                
                riggedRightArm.gameObject.SetActive(false);
                currentWeapon[0] =  hit.collider.GetComponent<WeaponInfromation>();
                currentWeapon[0].HandSetting(slotForWeapon[0],this);
                Reach = MathF.Max(currentWeapon[0].Reach, currentWeapon[1] == null ? 0: currentWeapon[1].Reach);
                Aoe = MathF.Max(currentWeapon[0].AreaOfEffect, currentWeapon[1] == null ? 0: currentWeapon[1].AreaOfEffect);
                
                rIsHolding = true;
            }
        }

        if (rIsHolding && rDrop)
        {
            rIsHolding = false;
            currentWeapon[0].DropSetting(this);
            riggedRightArm.gameObject.SetActive(true);
       
        }

        if (lIsHolding && lDrop)
        {
            lIsHolding = false;
            currentWeapon[1].DropSetting(this);
            riggedlefttArm.gameObject.SetActive(true);
        }

        lDrop = false;
        rDrop = false;
        rArm = false;
        lArm = false;
    }
}
