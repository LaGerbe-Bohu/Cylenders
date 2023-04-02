using UnityEngine;

[SerializeField]
public interface InterfaceInput
{
    public Vector3 movement();
    public Transform forwardTransform();
    public Transform renderForward();
    
    public bool jump();


}
