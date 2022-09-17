using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneCameraTrigger : MonoBehaviour
{

    [SerializeField]
    public Camera m_cCamera;
    
    [SerializeField]
    public int m_iZoneHeight;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        m_cCamera.transform.SetPositionAndRotation(Vector3.up * m_iZoneHeight, Quaternion.identity);
    }
}
