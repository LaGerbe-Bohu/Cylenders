using UnityEngine;
public class WeaponManager : MonoBehaviour
{
    /// <summary>
    /// Ce script va permettre de déctecter un armes, et de la prendre dans sa main
    /// </summary>
    // Start is called before the first frame update

    public Transform[] slotForWeapon; // ce sont les slots où les armes iront se mettre 
    public Transform Player;
    public Transform Camera;
    public LayerMask WeaponLayer;

    private WeaponInfromation[] currentWeapon;
    private bool isHolding;
    private bool Rarm;
    private bool Drop;

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
            Rarm = true;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Drop = true;
        }

        if (Input.GetMouseButton(0) && isHolding) 
        {
            currentWeapon[1].WeaponInterface.WeaponAction();
        }

    }

    void FixedUpdate()
    {
        RaycastHit hit;

        if (Physics.Raycast(Player.transform.position, Camera.transform.forward, out hit,10f,WeaponLayer))
        {
            
            if (Rarm)
            {
                currentWeapon[1] =  hit.collider.GetComponent<WeaponInfromation>();
                currentWeapon[1].HandSetting(slotForWeapon[1]);
                isHolding = true;
            }
        }

        if (isHolding && Drop)
        {
            isHolding = false;
            currentWeapon[1].DropSetting();
        }

        Drop = false;
        Rarm = false;

    }
}
