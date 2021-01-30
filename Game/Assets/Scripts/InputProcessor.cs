using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InputProcessor : MonoBehaviour
{
    private PlayerInput m_playerInput;
    private Vector2 m_movementInput;
    private bool m_isAiming;

    private EventHandler m_aimingStarted;
    private EventHandler m_aimingEnded;
    public event EventHandler AimingStarted
    {
        add => this.m_aimingStarted += value;
        remove => this.m_aimingStarted -= value;
    }
    
    public event EventHandler AimingEnded
    {
        add => this.m_aimingEnded += value;
        remove => this.m_aimingEnded -= value;
    }

    public Vector2 Movement => this.m_movementInput;
    public Vector2 MouseDelta { get; private set; }
    public bool ShootTriggered => this.m_playerInput.MainGame.Attack.triggered;
    public bool ReloadTriggered => this.m_playerInput.MainGame.Reload.triggered;

    private void Awake()
    {
        this.m_playerInput = new PlayerInput();
        this.m_playerInput.MainGame.Aim.started += context => this.m_aimingStarted?.Invoke(this, System.EventArgs.Empty);
        this.m_playerInput.MainGame.Aim.canceled += context => this.m_aimingEnded?.Invoke(this, System.EventArgs.Empty);
    }

    private void OnEnable()
    {
        this.m_playerInput?.Enable();
    }

    private void Update()
    {
        this.m_movementInput = this.m_playerInput.MainGame.Move.ReadValue<Vector2>();
        this.MouseDelta = this.m_playerInput.MainGame.Mouse.ReadValue<Vector2>();
    }

    private void OnDisable()
    {
        this.m_playerInput?.Disable();
        this.m_movementInput = Vector3.zero;
    }
}
