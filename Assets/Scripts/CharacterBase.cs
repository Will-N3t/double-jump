using UnityEngine;

public class CharacterBase : LivingThing
{

    [SerializeField]
    protected float m_fMaxMoveSpeed = 1f;
    [SerializeField]
    protected float m_fMoveAcceleration = 100f;
    
    [SerializeField]
    protected Rigidbody2D m_cRigidBody;
    
    [SerializeField]
    protected SpriteRenderer m_cSprite;
    
    [SerializeField] private Sprite m_cNormalSprite;
    [SerializeField] private Sprite m_cWalkSprite;
    [SerializeField] private Sprite m_cJumpSprite;
    [SerializeField] private Sprite m_cFallSprite;
    [SerializeField] private Sprite m_cStunSprite;

    protected bool m_bJumping = false;
    protected bool m_bFalling = false;
    protected bool m_bWalking = false;
    private bool m_bOldStunned = false;
    protected bool m_bStunned = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected virtual void SwitchStun(bool stun){}
    
    // Update is called once per frame
    protected void Update()
    {
        
        /////////////
        // Animation
        /////////////
        
        // Jump
        if (m_cRigidBody.velocity.y > 0.05f)
        {
            m_cSprite.sprite = m_cJumpSprite;
            m_bJumping = true;
        }
        else
        {
            m_bJumping = false;
        }
        
        // Fall
        if (m_cRigidBody.velocity.y < -0.05f)
        {
            m_cSprite.sprite = m_cFallSprite;
            m_bFalling = true;
        }
        else
        {
            m_bFalling = false;
        }
        
        // Walk
        if (!m_bJumping && !m_bFalling && (m_cRigidBody.velocity.x < -0.05f || m_cRigidBody.velocity.x > 0.05f))
        {
            m_cSprite.sprite = m_cWalkSprite;
            m_bWalking = true;
        }
        else
        {
            m_bWalking = false;
        }

        // Stun
        if (m_bStunned != m_bOldStunned)
        {
            m_bOldStunned = m_bStunned;
            SwitchStun(m_bStunned);
        }
        
        if (m_bStunned)
        {
            m_cSprite.sprite = m_cStunSprite;
        }

        if (!m_bFalling && !m_bJumping && !m_bWalking && !m_bStunned)
        {
            m_cSprite.sprite = m_cNormalSprite;
        }
        
        // Flip to direction of movement
        m_cSprite.flipX = m_cRigidBody.velocity.x < 0 || !(m_cRigidBody.velocity.x > 0) && m_cSprite.flipX ;
    }
}
