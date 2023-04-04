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
    public Animator SwordAnimator;


    private void Start()
    {
        WeaponInfromation = GetComponent<WeaponInfromation>();
    }

    public override void WeaponAction()
    {
        Debug.Log("Sword Attack");
        SwordAnimator.SetTrigger("Attack");

        RaycastHit hit;
        Transform camera = GameManager.instance.camera;
        Debug.DrawRay(camera.position, camera.forward * WeaponInfromation.reach,Color.magenta);
        if (Physics.Raycast(camera.position, camera.forward,out hit, WeaponInfromation.reach, WeaponInfromation.ennemiesLayer))
        {
            Debug.Log("hit");
            Destroy(hit.collider.gameObject);
        }

    }

    private void Update()
    {
       
    }
}
