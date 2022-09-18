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
    private float m_fJumpForce = 4f;
    
    [SerializeField]
    private bool m_bDisableInputOnBumpHead = true;
    [SerializeField]
    private CapsuleCollider2D head1;
    [SerializeField]
    private CapsuleCollider2D head2;
    
    [SerializeField]
    private CapsuleCollider2D feet1;
    [SerializeField]
    private CapsuleCollider2D feet2;

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
            m_cRigidBody.AddForce(Vector2.right * (m_fMoveSpeed * m_vMovement.x));
        }
        
        // Add jump force
        if (m_bJump)
        {
            // Manage being stunned
            if (m_bStunned)
            {
                Debug.Log("End Jump (Stunned)");
                m_bJump = false;
                return;
            }
            
            // Jump for a minimum amount
            if (!m_bJumpPressed && (m_cRigidBody.position.y > (m_fJumpStartHeight + m_fMinJumpHeight)))
            {
                Debug.Log("End Jump (Early Release)");
                m_bJump = false;
                return;
            }
            
            // End jump at max height
            if (m_cRigidBody.position.y >= m_fJumpStartHeight + m_fMaxJumpHeight)
            {
                Debug.Log("End Jump (Max Height)");
                m_bJump = false;
                return;
            }
            
            // Add force to get close to jump height
            //Debug.Log($"Jumping with power: [{m_fJumpForce * Square((m_fMaxJumpHeight - (m_cRigidBody.position.y - m_fJumpStartHeight))/m_fMaxJumpHeight)}] = m_fJumpForce[{m_fJumpForce}] * SQR((m_fMaxJumpHeight[{m_fMaxJumpHeight}] - (m_cRigidBody.position.y[{m_cRigidBody.position.y}] - m_fJumpStartHeight[{m_fJumpStartHeight}])[{m_cRigidBody.position.y - m_fJumpStartHeight}] )[{m_fMaxJumpHeight - (m_cRigidBody.position.y - m_fJumpStartHeight)}] / m_fMaxJumpHeight[{m_fMaxJumpHeight}] )[{(m_fMaxJumpHeight - (m_cRigidBody.position.y - m_fJumpStartHeight))/m_fMaxJumpHeight}] )[{Square(((m_fMaxJumpHeight - (m_cRigidBody.position.y - m_fJumpStartHeight))/m_fMaxJumpHeight))}]");
            var jumpPower = Square((m_fMaxJumpHeight - (m_cRigidBody.position.y - m_fJumpStartHeight)) / m_fMaxJumpHeight);
            
            Debug.Log($"Jump Power: {jumpPower}");
            m_cRigidBody.AddForce(Vector3.up * m_fJumpForce * jumpPower * Time.deltaTime);
        }

        float Square(float number)
        {
            return number * number * number;
        }
        
        // Feet
        var newFeetOnFloor = feet1.IsTouchingLayers() || feet2.IsTouchingLayers();
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
        
        // Animation
        base.Update();
        m_cMirrorSprite.sprite = m_cSprite.sprite;
        m_cMirrorSprite.flipX = m_cSprite.flipX;
    }
}
