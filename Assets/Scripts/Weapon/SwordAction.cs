using UnityEngine;

public class SwordAction : I_WeaponInterface
{
    /// <summary>
    /// Implémentation de la class abstract pour l'arme
    /// </summary>
    public override void WeaponAction()
    {
        Debug.Log("Sword Attack");
    }
    
}
