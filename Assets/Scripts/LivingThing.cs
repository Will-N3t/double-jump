using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingThing : MonoBehaviour
{
    [SerializeField]
    private int m_iHealth = 100;
    
    private bool m_bAlive;

    /// <summary>
    /// Health getter
    /// </summary>
    /// <returns>Current health value</returns>
    public int GetHealth()
    {
        return m_iHealth;
    }

    /// <summary>
    /// Deal damage to object, if health gets to zero then it will die
    /// </summary>
    /// <param name="damageValue">Damage to deal</param>
    /// <returns>Returns end health value</returns>
    public int Damage(int damageValue)
    {
        m_iHealth -= damageValue;

        // Kill if necessary
        if (m_iHealth <= 0)
        {
            m_iHealth = 0;
            m_bAlive = false;
            Die();
        }
        
        return GetHealth();
    }

    /// <summary>
    /// Handles death for this objectLivingThing
    /// </summary>
    protected virtual void Die()
    {
        Destroy(this, 2f);
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        m_bAlive = m_iHealth > 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
