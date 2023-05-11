using System;
using UnityEngine;
using UnityEngine.AI;

public class MobInput : MonoBehaviour,InterfaceInput
{

    public Transform orienteTransform;
    private Vector2 direction;

    public Vector2 getDirection()
    {
        return this.direction;
    }
    

    public void setDirection(Vector2 d )
    {
   
        this.direction = d;
    }
    
    public Vector2 movement()
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
