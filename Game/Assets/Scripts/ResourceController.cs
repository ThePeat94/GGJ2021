﻿using System;
using Nidavellir.FoxIt.EventArgs;
using Nidavellir.FoxIt.Scriptables;

namespace Nidavellir.FoxIt
{
    public class ResourceController
    {
        private EventHandler<ResourceValueChangedEventArgs> m_maxValueChanged;
        private EventHandler<ResourceValueChangedEventArgs> m_resourceValueChanged;

        public ResourceController(ResourceData data)
        {
            this.MaxValue = data.InitMaxValue;
            this.CurrentValue = data.StartValue;
        }

        public int CurrentValue { get; private set; }

        public int MaxValue { get; set; }

        public void Add(int value)
        {
            this.CurrentValue += value;
            this.m_resourceValueChanged?.Invoke(this, new ResourceValueChangedEventArgs(this.CurrentValue));
        }

        public bool CanAfford(int amount)
        {
            return this.CurrentValue > 0 && amount <= this.CurrentValue;
        }

        public void IncreaseMaximum(int value)
        {
            this.MaxValue += value;
            this.m_maxValueChanged?.Invoke(this, new ResourceValueChangedEventArgs(this.MaxValue));
        }

        public void ResetValue()
        {
            this.CurrentValue = this.MaxValue;
            this.m_resourceValueChanged?.Invoke(this, new ResourceValueChangedEventArgs(this.CurrentValue));
        }

        public void UseResource(int amount)
        {
            this.CurrentValue -= amount;
            this.m_resourceValueChanged?.Invoke(this, new ResourceValueChangedEventArgs(this.CurrentValue));
        }

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
    }
}