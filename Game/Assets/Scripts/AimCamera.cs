using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimCamera : MonoBehaviour
{
    [SerializeField] private InputProcessor m_playerInput;
    [SerializeField] private float m_maxUpwardsAngle;
    [SerializeField] private float m_maxDownwardsAngle;
    
    private float m_currentXRotation = 0f;
    
    private void Update()
    {
        this.m_currentXRotation -= this.m_playerInput.MouseDelta.y;
        this.m_currentXRotation = Mathf.Clamp(this.m_currentXRotation, this.m_maxDownwardsAngle, this.m_maxUpwardsAngle);
        this.transform.rotation = Quaternion.Euler(this.m_currentXRotation, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }
}
