using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : LivingThing
{

    [SerializeField]
    protected float m_fMoveSpeed = 0.5f;
    
    [SerializeField]
    protected Rigidbody2D m_cRigidBody;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
