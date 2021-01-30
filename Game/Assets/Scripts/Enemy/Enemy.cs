using System;
using System.Collections;
using System.Collections.Generic;
using Scriptables;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private ResourceData m_healthData;
    
    public ResourceController HealthController { get; private set; }

    private void Awake()
    {
        this.HealthController = new ResourceController(this.m_healthData);
    }
}
