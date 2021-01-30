using System;
using System.Collections;
using System.Collections.Generic;
using EventArgs;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private Slider m_healthBar;
    [SerializeField] private Enemy m_attachedEnemy;
    
    private void Start()
    {
        this.m_attachedEnemy.HealthController.ResourceValueChanged += HealthControllerOnResourceValueChanged;
        this.m_healthBar.maxValue = this.m_attachedEnemy.HealthController.MaxValue;
        this.m_healthBar.minValue = 0;
        this.m_healthBar.value = this.m_attachedEnemy.HealthController.CurrentValue;
    }

    private void HealthControllerOnResourceValueChanged(object sender, ResourceValueChangedEventArgs e)
    {
        this.m_healthBar.value = e.NewValue;
    }
}
