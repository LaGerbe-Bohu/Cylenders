using System;
using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(WeaponInfromation))]
public class SwordAction : I_WeaponInterface
{
    /// <summary>
    /// Implémentation de la class abstract pour l'arme
    /// </summary>
    /// 

    public float Dommage = 1;
    public float Reach;
    private WeaponInfromation WeaponInfromation;
    
    private void Start()
    {
        WeaponInfromation = GetComponent<WeaponInfromation>();
    }

    public override void WeaponAction()
    {

        RaycastHit hit;
        Transform camera = GameManager.instance.cameraPlayer;
        if (Physics.Raycast(camera.position, camera.forward,out hit, GameManager.instance.PlayerReach, WeaponInfromation.ennemiesLayer))
        {
           
            EnnemieInformation EI = hit.collider.GetComponent<EnnemieInformation>();

            if (EI)
            {
                EI.life -= Dommage;
                EI.Hurt(this.transform.position);
                if (EI.life <= 0)
                {
                    Destroy(EI.gameObject);
                }
            }

        }

    }

    private void Update()
    {
       
    }
}
