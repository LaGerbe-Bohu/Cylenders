using System;
using UnityEngine;

public class SwordAction : I_WeaponInterface
{
    /// <summary>
    /// Implémentation de la class abstract pour l'arme
    /// </summary>
    /// 

    public Animator SwordAnimator;
    public override void WeaponAction()
    {
        Debug.Log("Sword Attack");
        SwordAnimator.SetBool("Attack",true);
    }

    private void Update()
    {
        SwordAnimator.SetBool("Attack",false);
    }
}
