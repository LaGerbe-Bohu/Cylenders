using System;
using UnityEngine;
using UnityEngine.AI;

public class MobInput : MonoBehaviour,InterfaceInput
{

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

    public Vector3 forwardTransform()
    {
        return this.transform.forward;
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
