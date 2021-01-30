using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody m_rigidbody;
    private Collider m_collider;

    private bool m_hasHit;

    public int Damage { get; set; }
    
    private void Awake()
    {
        this.m_collider = this.GetComponent<Collider>();
        this.m_rigidbody = this.GetComponent<Rigidbody>();
        Destroy(this,5f);
    }
    
    // Update is called once per frame
    void Update()
    {
        if(!this.m_hasHit)
            this.transform.rotation = Quaternion.LookRotation(this.m_rigidbody.velocity);
    }

    private void OnCollisionEnter(Collision other)
    {
        this.m_rigidbody.velocity = Vector3.zero;
        this.m_rigidbody.isKinematic = true;
        this.transform.SetParent(other.transform);
        this.m_hasHit = true;
        this.m_collider.enabled = false;
    }
}
