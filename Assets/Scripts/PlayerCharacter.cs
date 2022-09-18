using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerCharacter : CharacterBase
{
    private Vector2 m_vMovement = Vector2.zero;

    [SerializeField]
    private float m_fJumpForce = 500000f;
    
    [SerializeField]
    private bool m_bDisableInputOnBumpHead = true;
    [SerializeField]
    private CapsuleCollider2D jumpStopper1;
    [SerializeField]
    private CapsuleCollider2D jumpStopper2;
    [SerializeField]
    private CapsuleCollider2D head1;
    [SerializeField]
    private CapsuleCollider2D head2;
    
    [SerializeField]
    private CapsuleCollider2D feet1;
    [SerializeField]
    private CapsuleCollider2D feet2;

    [SerializeField]
    private float m_fMaxJumpTime = 1f;
    private float m_fJumpTime = 0f;
    [SerializeField]
    private float m_fMaxJumpHeight = 3f;
    [SerializeField]
    private float m_fMinJumpHeight = 2.5f;
    private float m_fJumpStartHeight = 0f;
    
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
    private AudioSource m_cHitHeadSound;
    [SerializeField]
    private AudioSource m_cHitGroundSound;
    

    public void Jump(InputAction.CallbackContext context)
    {
        bool jumpButton = context.ReadValueAsButton();
        
        //Debug.Log($"Jump Pressed: jumpButton[{jumpButton}] | m_bFeetOnFloor [{m_bFeetOnFloor}] | !m_bJump[{!m_bJump}] | !m_bJumpPressed [{!m_bJumpPressed}] | !m_bStunned[{!m_bStunned}] ");
        
        // Start Jump
        if (jumpButton && (m_bFeetOnFloor || m_bDEBUGInfinityJump) && !m_bJump && !m_bJumpPressed && !m_bStunned)
        {
            m_cJumpSound.Play();
            m_bJump = true;
            m_fJumpTime = m_fMaxJumpTime;
            m_fJumpStartHeight = m_cRigidBody.position.y;
        }

        m_bJumpPressed = jumpButton;
    }
        
    // Start is called before the first frame update
    void Start()
    {
        m_cJumpSound.clip.LoadAudioData();
        m_cHitHeadSound.clip.LoadAudioData();
        m_cHitGroundSound.clip.LoadAudioData();
    }

    public void PlayerMove(InputAction.CallbackContext context)
    {
        m_vMovement = context.ReadValue<Vector2>();
    }

    // Update is called once per frame
    new void Update()
    {
        // Add movement force
        if (!m_bStunned)
        {
            m_cRigidBody.AddForce(Vector2.right * (m_fMoveSpeed * m_vMovement.x) * Time.deltaTime);
        }
        
        // Add jump force
        if (m_bJump)
        {
            // Manage being stunned
            if (m_bStunned)
            {
                m_bJump = false;
                return;
            }
            
            // Jump for a minimum amount
            if (!m_bJumpPressed && (m_cRigidBody.position.y > (m_fJumpStartHeight + m_fMinJumpHeight)))
            {
                m_bJump = false;
                return;
            }
            
            // End jump at max height
            if (m_cRigidBody.position.y >= m_fJumpStartHeight + m_fMaxJumpHeight)
            {
                m_bJump = false;
                return;
            }

            // End jump after max time to stop floating
            m_fJumpTime -= Time.deltaTime;
            if (m_fJumpTime <= 0f)
            {
                Debug.Log("Hit max jump time");
                m_bJump = false;
                return;
            }
            
            // Add force to get close to jump height
            var jumpPower = Power((m_fMaxJumpHeight - (m_cRigidBody.position.y - m_fJumpStartHeight)) / (m_fMaxJumpHeight*3));
            
            //Debug.Log($"Jump Power: {jumpPower}");
            m_cRigidBody.AddForce(Vector3.up * m_fJumpForce * jumpPower * Time.deltaTime);
        }

        float Power(float number, uint power = 2)
        {
            for (int i = 0; i < power; i++)
            {
                number *= number;
            }
            
            return number;
        }
        
        // Feet
        var newFeetOnFloor = (feet1.IsTouchingLayers() && !feet1.IsTouchingLayers(2)) || (feet2.IsTouchingLayers() && !feet1.IsTouchingLayers(2));
        if (!m_bFeetOnFloor && newFeetOnFloor)
        {
            m_cHitGroundSound.Play();

            // Re-enable input once stun finished
            if (m_cRigidBody.velocity.magnitude < 0.01f)
            {
                m_bStunned = false;
            }
        }
        m_bFeetOnFloor = newFeetOnFloor;

        // Bump Head
        if (!m_bStunned && m_bDisableInputOnBumpHead && (head1.IsTouchingLayers() || head2.IsTouchingLayers()))
        {
            m_bStunned = true;
            m_cHitHeadSound.Play();
            m_bJump = false;
            m_vMovement = Vector2.zero;
        }
        
        // Stop Jump
        if (jumpStopper1.IsTouchingLayers() || jumpStopper2.IsTouchingLayers())
        {
            m_bJump = false;
        }
        
        // Animation
        base.Update();
        m_cMirrorSprite.sprite = m_cSprite.sprite;
        m_cMirrorSprite.flipX = m_cSprite.flipX;
    }
}
