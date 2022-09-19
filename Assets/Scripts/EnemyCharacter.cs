using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : CharacterBase
{
    [SerializeField]
    private string m_sPlayerTag = "Player";
    [SerializeField]
    private float m_fJumpPower = 20000f;
    [SerializeField]
    private float m_fGrowSpeed = 3f;
    [SerializeField]
    private float m_fGrowToSize = 3f;
    [SerializeField]
    private float m_fMoveDistance = 2f;
    private float m_fStartPosition = 0f;
    private float m_fTargetPosition = 0f;
    private int m_fDirection = -1;
    
    [SerializeField]
    private GameObject m_cEnemy1;
    [SerializeField]
    private GameObject m_cEnemy2;
    [SerializeField]
    private Collider2D m_cMainCollider1;
    [SerializeField]
    private Collider2D m_cMainCollider2;
    [SerializeField]
    private SpriteRenderer m_cMirrorSprite;

    [SerializeField]
    private AudioSource m_cDieSound;

    [SerializeField]
    private Game m_cGame;
    

        
    // Start is called before the first frame update
    void Start()
    {
        m_cDieSound.clip.LoadAudioData();
        m_fStartPosition = m_cRigidBody.position.x;
        m_fTargetPosition = m_fStartPosition + (m_fMoveDistance * m_fDirection);
    }

    
    // Update is called once per frame
    new void Update()
    {
        
        // Add movement force
        if (!m_bStunned)
        {
            // Switch Direction
            if (Mathf.Abs(m_fTargetPosition - m_cRigidBody.position.x) < 0.05f )
            {
                m_fDirection *= -1;
                
                m_fTargetPosition = m_fStartPosition + (m_fMoveDistance * m_fDirection);
                
            }
                
            m_cRigidBody.AddForce(Vector2.right * m_fDirection * m_fMoveAcceleration * Time.deltaTime);
        }
        else
        {
            if (m_cEnemy1.transform.localScale.x < m_fGrowToSize)
            {
                m_cEnemy1.transform.localScale += Vector3.one * m_fGrowSpeed * Time.deltaTime;
                m_cEnemy2.transform.localScale += Vector3.one * m_fGrowSpeed * Time.deltaTime;
            }
        }
        
        // Check for Kill
        void CheckKill(int numContacts, ref List<Collider2D> contacts)
        {
            for (int i = 0; i < numContacts; i++)
            {
                if (contacts[i] != null && contacts[i].gameObject.CompareTag(m_sPlayerTag))
                {
                    m_bStunned = true;
                }
            }
        }
        
        var contacts1 = new List<Collider2D>(2);
        var contacts2 = new List<Collider2D>(2);
        var numContacts1 = m_cMainCollider1.GetContacts(contacts1);
        var numContacts2 = m_cMainCollider1.GetContacts(contacts2);
        CheckKill(numContacts1, ref contacts1);
        CheckKill(numContacts2, ref contacts2);
        
        
        // Animation
        base.Update();
        m_cMirrorSprite.sprite = m_cSprite.sprite;
        m_cMirrorSprite.flipX = m_cSprite.flipX;
    }
    /// <summary>
    /// Override method for customising behaviour on stun
    /// Triggered by parent class detection of stun / unstun of <code>m_bStunned</code> value
    /// </summary>
    /// <param name="stun">The new stun value passed in</param>
    protected override void SwitchStun(bool stun)
    {
        // Colliders
        m_cMainCollider1.enabled = !stun;
        m_cMainCollider2.enabled = !stun;
        
        if (stun)
        {
            // Physics
            m_cRigidBody.mass = 0.1f;
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -5);
            m_cRigidBody.AddForce(Vector2.up * m_fJumpPower);
            
            // Sound
            m_cDieSound.Play();
            
            m_cGame.TriggerWin();
            Destroy(this, 5f);
        }
    }
    
    /// <summary>
    /// Deal with triggers
    /// </summary>
    /// <param name="other">The trigger touched</param>
    private void Coll (Collider other)
    {
        
    }
}
