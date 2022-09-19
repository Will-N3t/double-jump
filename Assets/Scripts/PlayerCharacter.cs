using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : CharacterBase
{
    private Vector2 m_vMovement = Vector2.zero;
    private string StunTag = "Stun";
    private int NoJumpLayer = 2;

    [SerializeField]
    private float m_fNormalMass = 1f;
    [SerializeField]
    private float m_fStunMass = 0f;
    
    [SerializeField]
    private float m_fJumpForce = 500000f;
    
    [SerializeField]
    private bool m_bDisableInputOnBumpHead = true;
    [SerializeField]
    private Collider2D jumpStopper1;
    [SerializeField]
    private Collider2D jumpStopper2;
    [SerializeField]
    private Collider2D stunCollider1;
    [SerializeField]
    private Collider2D stunCollider2;
    [SerializeField]
    private Collider2D mainCollider1;
    [SerializeField]
    private Collider2D mainCollider2;
    [SerializeField]
    private Collider2D head1;
    [SerializeField]
    private Collider2D head2;
    
    [SerializeField]
    private Collider2D feet1;
    [SerializeField]
    private Collider2D feet2;

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
    [SerializeField]
    private AudioSource m_cStunnedHitGroundSound;
    [SerializeField]
    private AudioSource m_cSlipSound;
    

    /// <summary>
    /// Basic movement input function
    /// Method triggered by input controller
    /// </summary>
    /// <param name="context"></param>
    public void PlayerMove(InputAction.CallbackContext context)
    {
        m_vMovement = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Jump function
    /// Method triggered by input controller
    /// </summary>
    /// <param name="context">Input context</param>
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

    /// <summary>
    /// Override method for customising behaviour on stun
    /// Triggered by parent class detection of stun / unstun of <code>m_bStunned</code> value
    /// </summary>
    /// <param name="stun">The new stun value passed in</param>
    protected override void SwitchStun(bool stun)
    {
        Debug.Log($"Stun: {stun}");
        
        // Colliders
        stunCollider1.enabled = stun;
        stunCollider2.enabled = stun;
        mainCollider1.enabled = !stun;
        mainCollider2.enabled = !stun;
        jumpStopper1.enabled = !stun;
        jumpStopper2.enabled = !stun;
        head1.enabled = !stun;
        head2.enabled = !stun;
        feet1.enabled = !stun;
        feet2.enabled = !stun;
        
        // Physics
        m_cRigidBody.mass = stun ? m_fStunMass : m_fNormalMass;
    }
    
    /// <summary>
    /// Power function
    /// Basically Mathf.Pow
    /// </summary>
    /// <param name="number">Number to power</param>
    /// <param name="power">Power to reach</param>
    /// <returns>Number to the Power</returns>
    static float Power(float number, uint power = 2)
    {
        for (int i = 0; i < power; i++)
        {
            number *= number;
        }

        return number;
    }

    /// <summary>
    /// Exponentially tweens a multiplier value down as it reaches its target
    /// </summary>
    /// <param name="current"></param>
    /// <param name="target"></param>
    /// <returns>0 - 1 multiplier</returns>
    static float ForceMultiplierToTarget(float current, float target)
    {
        return Mathf.Pow((target - current) / (target*3), 2);
    }
    
    // Update is called once per frame
    new void Update()
    {
        
        // Add jump force
        if (m_bJump)
        {
            // Stop jump if stunned
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
                m_bJump = false;
                return;
            }
            
            // Add force to get close to jump height
            
            var jumpPower = Power((m_fMaxJumpHeight - (m_cRigidBody.position.y - m_fJumpStartHeight)) / (m_fMaxJumpHeight*3));
            
            m_cRigidBody.AddForce(Vector3.up * m_fJumpForce * jumpPower * Time.deltaTime);
        }

        // Feet
        void CheckFeet(int numContacts, ref List<Collider2D> contacts, ref bool feetOnFloor)
        {
            for (int i = 0; i < numContacts; i++)
            {
                if (contacts[i] != null && contacts[i].gameObject.layer != NoJumpLayer)
                {
                    feetOnFloor = true;
                }
                // Slip on slope
                else if (contacts[i] != null)
                {
                    m_cSlipSound.Play();
                    m_bStunned = true;
                }
            }
        }
        
        bool newFeetOnFloor = false;
        var contacts1 = new List<Collider2D>(2);
        var contacts2 = new List<Collider2D>(2);
        var numContacts1 = feet1.GetContacts(contacts1);
        var numContacts2 = feet2.GetContacts(contacts2);
        CheckFeet(numContacts1, ref contacts1, ref newFeetOnFloor);
        CheckFeet(numContacts2, ref contacts2, ref newFeetOnFloor);
        
        if (!m_bFeetOnFloor && newFeetOnFloor)
        {
            m_cHitGroundSound.Play();
            m_bJump = false;
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
        
        
        // Re-enable input once stun finished
        if (m_bStunned && (stunCollider1.IsTouchingLayers() || stunCollider2.IsTouchingLayers()) && m_cRigidBody.velocity.magnitude < 0.005f)
        {
            Debug.Log("Out of Stun!");
            m_bStunned = false;
            m_cStunnedHitGroundSound.Play();
        }
        
        
        // Add movement force
        if (!m_bStunned)
        {
            
            // Don't add movement power if the main body is touching something but the feet aren't
            if (!(!m_bFeetOnFloor && (mainCollider1.IsTouchingLayers() || mainCollider2.IsTouchingLayers())))
            {
                var movePower = ForceMultiplierToTarget(Mathf.Abs(m_cRigidBody.velocity.x), m_fMaxMoveSpeed);
                m_cRigidBody.AddForce(Vector2.right * m_vMovement.x * m_fMoveAcceleration * movePower * Time.deltaTime);
            }
        }
        else
        {
            if (feet1.IsTouchingLayers() || feet2.IsTouchingLayers())
            {
                m_cHitGroundSound.Play();
            }
        }
        
        
        // Animation
        base.Update();
        m_cMirrorSprite.sprite = m_cSprite.sprite;
        m_cMirrorSprite.flipX = m_cSprite.flipX;
    }
    
    /// <summary>
    /// Deal with triggers
    /// </summary>
    /// <param name="other">The trigger touched</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(StunTag))
        {
            m_bStunned = true;
        }
    }
}
