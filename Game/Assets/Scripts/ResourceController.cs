using System;
using System.Collections;
using System.Collections.Generic;
using EventArgs;
using Scriptables;
using UnityEngine;

public class ResourceController
{
    private int m_currentValue;
    private EventHandler<ResourceValueChangedEventArgs> m_resourceValueChanged;
    private EventHandler<ResourceValueChangedEventArgs> m_maxValueChanged;
    
    public ResourceController(ResourceData data)
    {
        this.MaxValue = data.InitMaxValue;
        this.m_currentValue = data.StartValue;
    }
    
    public int CurrentValue => this.m_currentValue;
    public int MaxValue { get; set; }
    
    public event EventHandler<ResourceValueChangedEventArgs> ResourceValueChanged
    {
        add => this.m_resourceValueChanged += value;
        remove => this.m_resourceValueChanged -= value;
    }
    
    public event EventHandler<ResourceValueChangedEventArgs> MaxValueChanged
    {
        add => this.m_maxValueChanged += value;
        remove => this.m_maxValueChanged -= value;
    }
    
    public void ResetValue()
    {
        this.m_currentValue = this.MaxValue;
        this.m_resourceValueChanged?.Invoke(this, new ResourceValueChangedEventArgs(this.m_currentValue));
    }
    
    public void UseResource(int amount)
    {
        this.m_currentValue -= amount;
        this.m_resourceValueChanged?.Invoke(this, new ResourceValueChangedEventArgs(this.m_currentValue));
    }
    
    public bool CanAfford(int amount)
    {
        return this.m_currentValue > 0 && amount <= this.m_currentValue;
    }

    public void Add(int value)
    {
        this.m_currentValue += value;
        this.m_resourceValueChanged?.Invoke(this, new ResourceValueChangedEventArgs(this.m_currentValue));
    }

    public void IncreaseMaximum(int value)
    {
        this.MaxValue += value;
        this.m_maxValueChanged?.Invoke(this, new ResourceValueChangedEventArgs(this.MaxValue));
    }
}
