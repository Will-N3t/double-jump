using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : CharacterBase
{
    private float m_fLeftOrRight = 0;

    [SerializeField]
    private float m_fJumpForce = 500;
    
    [SerializeField]
    private CapsuleCollider2D feet;
    
    private bool m_bFalling = false;
    
    private int m_iLevel = 0;

    private int m_iLevelSize = 10;

    [SerializeField]
    private Camera m_cCamera;

    public void Jump()
    {
        Debug.Log("Jump?");
        
        if (!m_bFalling)
        {
            Debug.Log("Jump!");
            m_cRigidBody.AddForce(Vector3.up * m_fJumpForce);
        }
    }
        
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PlayerMove(InputAction.CallbackContext context)
    {
        var v = context.ReadValue<Vector2>();

        m_fLeftOrRight = v.x;

        if (v.y >= 0.9f)
        {
            Jump();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Add movement force
        m_cRigidBody.AddForce(Vector2.right * (m_fMoveSpeed * m_fLeftOrRight));
        
        // Update Cam
        if (m_iLevel != GetLevelFromPosition())
        {
            m_iLevel = GetLevelFromPosition();
            m_cCamera.transform.SetPositionAndRotation(new Vector3(0, m_iLevel*m_iLevelSize, -10), Quaternion.identity);
            
            Debug.Log("New Level: " + m_iLevel);
        }
    }

    private int GetLevelFromPosition()
    {
        return Mathf.FloorToInt((m_cRigidBody.position.y + (m_iLevelSize/2f)) / m_iLevelSize);
    }
    
    void OnCollisionStay2D(Collision2D collisionInfo)
    {
        m_bFalling = collisionInfo.contacts.Length == 0;
        Debug.Log("Collisions: " + collisionInfo.contacts.Length);
        
        // Debug-draw all contact points and normals
        foreach (ContactPoint2D contact in collisionInfo.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal * 10, Color.green);
        }
    }
}
