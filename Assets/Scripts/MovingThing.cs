using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingThing : MonoBehaviour
{
    [SerializeField]
    protected float m_fDragCE;
    
    public Vector3 m_vVelocity;

    
    public void SetDrag(float drag)
    {
        m_fDragCE = drag;
    }
    
    public float GetDrag()
    {
        return m_fDragCE;
    }

    public void AddForce(Vector3 force)
    {
        m_vVelocity += force;
    }

    public void SetForce(Vector3 force)
    {
        m_vVelocity = force;
    }

    public Vector3 GetVelocity()
    {
        return m_vVelocity;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Slow with drag
        m_vVelocity *= (1 - m_fDragCE);
        
        // Lock Z axis
        m_vVelocity.z = 0;
        
        // Update position
        this.transform.position += ( m_vVelocity + ( Vector3.down * 9.81f ) ) * Time.deltaTime;
    }
}
