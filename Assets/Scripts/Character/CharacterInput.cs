using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInput : MonoBehaviour,InterfaceInput
{
    /// <summary>
    /// This script is a implementation of interface that is give to Charactercontroll to handle input
    /// </summary>
    
    [Header("Compenents")] 
    public Transform forwTransform;
    public Transform renderTransform;
    public PlayerInput playerInput;

    [Header("Values")] 
    public string DeplacementName;
    public string JumpName;
    
    public Vector2 movement()
    {
        return playerInput.actions[DeplacementName].ReadValue<Vector2>();;
    }
    public Transform renderForward()
    {
        return this.renderTransform;
    }

    public bool jump()
    {
        return playerInput.actions[JumpName].IsPressed();
    }

    public Transform forwardTransform()
    {
        return this.forwTransform;
    }
    
}
