using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class WeaponTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void GrabWeaponTest()
    {
        SceneManager.LoadScene("WeaponUnitScene");
        WeaponManager WM = GameManager.instance.getPlayer().GetComponent<WeaponManager>();
    }
    
}
