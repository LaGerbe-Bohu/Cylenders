using UnityEngine;
using UnityEngine.Serialization;

public class WeaponManager : MonoBehaviour
{
    /// <summary>
    /// Ce script va permettre de déctecter un armes, et de la prendre dans sa main
    /// </summary>
    // Start is called before the first frame update

    public Transform[] slotForWeapon; // ce sont les slots où les armes iront se mettre 
    [FormerlySerializedAs("Player")] public Transform player;
    [FormerlySerializedAs("Camera")] public Transform camera;
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

        if (Input.GetMouseButton(0) && rIsHolding && currentWeapon[0] != null) 
        {
            currentWeapon[0].WeaponInterface.WeaponAction();
        }
        
        if (Input.GetMouseButton(1) && lIsHolding && currentWeapon[1] != null) 
        {
            currentWeapon[1].WeaponInterface.WeaponAction();
        }

    }

    void FixedUpdate()
    {
        RaycastHit hit;

        if (Physics.Raycast(player.transform.position, camera.transform.forward, out hit,10f,weaponLayer))
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
