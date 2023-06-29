using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


[RequireComponent(typeof(InterfaceInput))]
public class CharacterController : MonoBehaviour
{
    ///
    /// This script is use to handle movement for the main character
    ///
    /// 
    [Header("Compenents")] 
    public Rigidbody rigidBody;
    public InterfaceInput interfaceInput;
    
    
    [Header("Movement Values")]
    public float acceleration;
    public float maxSpeed;
    public float jumpForce;
    public float AirControlScaler = 0.1f;
    public LayerMask GroundLayer;
    [Space] 
    public bool airControl = false;
    public float maxSlopeAngle = 45f;

    [Space] 
    public Animator RightHand;
    public Animator LefttHand;
    
    public bool orienteCharacter;
    // some private var
    private Vector2 _direction;
    private bool _jump;
    private bool _isGrounded;
    private Vector3 _normalSurface;
    private Vector3 _tangentSurface;
    private bool  animated = false;

    void Start()
    {
        this.interfaceInput = GetComponent<InterfaceInput>();

        if (RightHand != null && LefttHand != null)
        {
            animated = true;
        }
    }
    
    void Update()
    {
        // Inputs
        _direction = interfaceInput.movement();
        _jump = interfaceInput.jump();
        
        OrientCharacter();
    }

    private void FixedUpdate()
    {
        MoveCharacter();
        JumpCharacter();
        IsGrounded();
    }
    
    
    void MoveCharacter()
    {
       
        if (!airControl && !_isGrounded) return;
        float scaler = this.AirControlScaler;
        
        if (!airControl || _isGrounded)
        {
            scaler = 1.0f;
        }

      
        
        // move character
        var biaisTransform = interfaceInput.renderForward();
        float angle = ProjectAngle(_tangentSurface,Vector3.forward,biaisTransform.right);
        Vector3 forward = biaisTransform.forward;
        
        if (angle <= maxSlopeAngle)
        {
            forward = (_tangentSurface + biaisTransform.forward).normalized;
            if (angle < 0 && _direction.y > 0)
            {
                forward = (_tangentSurface +( -biaisTransform.up)).normalized;
            }
            else if (angle > 0 && _direction.y < 0)
            {
                forward = _tangentSurface;
            }
        }
        
        Vector3 right =  biaisTransform.right;

        
        if (animated)
        {
            RightHand.SetBool("Walk",false);
            LefttHand.SetBool("Walk",false);
        }

        
        // Movement
        if (_direction.magnitude > float.Epsilon)
        {
            Vector3 move = (forward * (_direction.y ) + right * (_direction.x )).normalized * (acceleration * scaler);
            rigidBody.AddForce(move,ForceMode.Impulse); 
            
              
            if (animated && _isGrounded )
            {
                RightHand.SetBool("Walk",true);
                LefttHand.SetBool("Walk",true);
              
            }

        }



        // Normalise speed
        var velocity = rigidBody.velocity;
        float tmpY = velocity.y;
        
        Vector3 tmpVelo;
        tmpVelo = Vector3.ClampMagnitude(velocity, maxSpeed);
        tmpVelo = new Vector3(tmpVelo.x, tmpY, tmpVelo.z);
        rigidBody.velocity = tmpVelo;
        
    }



    
    void OrientCharacter()
    {
        if (!orienteCharacter) return;
        var up = rigidBody.transform.up;
        Vector3 forward = Vector3.ProjectOnPlane(interfaceInput.forwardTransform().forward, up);
        
        interfaceInput.renderForward().transform.LookAt(this.rigidBody.transform.position + forward*2);
        
    }
    
    private void JumpCharacter()
    {
        if (!_jump || !_isGrounded) return;

        
        rigidBody.AddForce(rigidBody.transform.up*jumpForce,ForceMode.VelocityChange);
    }
    
    void IsGrounded()
    {
        RaycastHit hit;

        Vector3 postiion = rigidBody.transform.position + Vector3.up*0.35f;

        _isGrounded = false;
        _normalSurface = interfaceInput.renderForward().up;
        _tangentSurface = interfaceInput.renderForward().forward;

        if (_jump) return;
        if (Physics.SphereCast(postiion, 0.8f, -rigidBody.transform.up, out hit, 1f,GroundLayer))
        {
            _normalSurface = hit.normal;
            _tangentSurface = Vector3.Cross( interfaceInput.renderForward().transform.right,hit.normal);
            _isGrounded =  true;
            
            _normalSurface = _normalSurface.normalized;
            _tangentSurface = _tangentSurface.normalized;

        }
        
    }

    private void OnDrawGizmos()
    {
        
        Gizmos.color = Color.blue;
        var transform1 = rigidBody.transform;
        Gizmos.DrawRay(transform1.position + Vector3.up*0.35f, -transform1.up*1f);
        
        Gizmos.color = Color.red;
        var transform2 = rigidBody.transform;
        Gizmos.DrawWireSphere((transform2.position + Vector3.up*.35f) -transform2.up*1f ,.8f);
        
        Gizmos.color = Color.green;
        var position = this.transform.position;
        Gizmos.DrawRay(position,_normalSurface*10f);
        Gizmos.DrawRay(position,_tangentSurface*10f);
        
    }
    
    
    // useful fonction
    public float ProjectAngle(Vector3 A, Vector3 B, Vector3 normal)
    {
        Vector3 a = Vector3.ProjectOnPlane(A, normal);
        Vector3 b = Vector3.ProjectOnPlane(B, normal);

        return Vector3.SignedAngle(a, b,Vector3.right);
    }
    
}
