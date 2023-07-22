using UnityEngine;

[SerializeField]
public interface InterfaceInput
{
    public Vector2 movement();
    public Vector3 forwardTransform();
    public Transform renderForward();
    
    public bool jump();


}
