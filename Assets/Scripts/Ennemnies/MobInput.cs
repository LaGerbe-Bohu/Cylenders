using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobInput : MonoBehaviour,InterfaceInput
{

    public Transform orienteTransform;
    private Vector3 direction;


    public Vector3 getDirection()
    {
        return this.direction;
    }
    

    public void setDirection(Vector3 d )
    {
        this.direction = d;
    }
    
    public Vector3 movement()
    {
        return direction;
    }

    public Transform forwardTransform()
    {
        return this.transform;
    }

    public Transform renderForward()
    {
        return this.transform;
    }

    public bool jump()
    {
        return false;
    }
    
}
