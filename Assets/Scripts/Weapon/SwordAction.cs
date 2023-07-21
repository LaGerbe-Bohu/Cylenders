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

  
    private WeaponInfromation WeaponInfromation;
    
    private void Start()
    {
        WeaponInfromation = GetComponent<WeaponInfromation>();
    }

    public override void WeaponAction()
    {

        RaycastHit hit;
        Transform camera = GameManager.instance.cameraPlayer;

        var Ennemies = Physics.SphereCastAll(camera.position, WeaponInfromation.AreaOfEffect, camera.forward, WeaponInfromation.Reach, WeaponInfromation.ennemiesLayer);

        for (int i = 0; i < Ennemies.Length; i++)
        {
            EnnemieInformation EI = Ennemies[i].collider.GetComponent<EnnemieInformation>();

            if (EI)
            {
                EI.life -= WeaponInfromation.Dommage;
                EI.Hurt(this.transform.position, WeaponInfromation);
                if (EI.life <= 0)
                {
                   EI.Killed();
                }
            }
            
        }

    }

    private void Update()
    {
       
    }
}
