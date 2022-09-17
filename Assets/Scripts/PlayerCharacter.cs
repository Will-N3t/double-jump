using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : CharacterBase
{
    private Vector2 m_vMovement = Vector2.zero;

    [SerializeField]
    private float m_fJumpForce = 1.5f;
    
    [SerializeField]
    private CapsuleCollider2D feet1;
    [SerializeField]
    private CapsuleCollider2D feet2;

    [SerializeField]
    private float m_fMaxJumpTime = 0.3f;
    [SerializeField]
    private float m_fMinJumpTime = 0.2f;
    private float m_fJumpTime = 0f;
    
    [SerializeField]
    private bool m_bDEBUGInfinityJump = false;
    private bool m_bJumpPressed = false;
    private bool m_bJump = false;
    private bool m_bFeetOnFloor = false;
    
    [SerializeField]
    private SpriteRenderer m_cMirrorSprite;

    [SerializeField]
    private AudioSource m_cJumpSound;
    [SerializeField]
    private AudioSource m_cHitSound;
    

    public void Jump(InputAction.CallbackContext context)
    {
        bool jumpButton = context.ReadValueAsButton();
        
        // Start Jump
        if ((m_bFeetOnFloor || m_bDEBUGInfinityJump) && !m_bJump && !m_bJumpPressed && jumpButton)
        {
            m_cJumpSound.Play();
            m_bJump = true;
            m_fJumpTime = m_fMaxJumpTime;
        }

        m_bJumpPressed = jumpButton;
    }
        
    // Start is called before the first frame update
    void Start()
    {
        m_cJumpSound.clip.LoadAudioData();
        m_cHitSound.clip.LoadAudioData();
    }

    public void PlayerMove(InputAction.CallbackContext context)
    {
        m_vMovement = context.ReadValue<Vector2>();
    }

    // Update is called once per frame
    new void Update()
    {
        // Add movement force
        m_cRigidBody.AddForce(Vector2.right * (m_fMoveSpeed * m_vMovement.x));
        
        // Add jump force
        if (m_bJump)
        {
            // Decrease the jump time thus far
            m_fJumpTime -= Time.deltaTime;
            
            if (m_fJumpTime <= 0)
            {
                m_bJump = false;
            }

            // Jump for a minimum amount
            if (!m_bJumpPressed && (m_fMaxJumpTime - m_fJumpTime) > m_fMinJumpTime)
            {
                m_bJump = false;
            }
            
            m_cRigidBody.AddForce(Vector3.up * m_fJumpForce * (m_fJumpTime/m_fMaxJumpTime));
        }
        
        // Feet
        var newFeetOnFloor = feet1.IsTouchingLayers() || feet2.IsTouchingLayers();
        if (!m_bFeetOnFloor && newFeetOnFloor)
        {
            m_cHitSound.Play();
        }
        m_bFeetOnFloor = newFeetOnFloor;
        
        // Animation
        base.Update();
        m_cMirrorSprite.sprite = m_cSprite.sprite;
        m_cMirrorSprite.flipX = m_cSprite.flipX;
    }
}
