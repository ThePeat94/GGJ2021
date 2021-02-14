using System;
using UnityEngine;

namespace Nidavellir.FoxIt.InputController
{
    public class GameInputProcessor : MonoBehaviour
    {
        private EventHandler m_aimingEnded;

        private EventHandler m_aimingStarted;
        private bool m_isAiming;
        private PlayerInput m_playerInput;

        public Vector2 Movement { get; private set; }

        public Vector2 MouseDelta { get; private set; }
        public bool ShootTriggered => this.m_playerInput.MainGame.Attack.triggered;
        public bool ReloadTriggered => this.m_playerInput.MainGame.Reload.triggered;
        public bool QuitTriggered => this.m_playerInput.MainGame.Quit.triggered;
        public bool InteractTriggered => this.m_playerInput.MainGame.Interact.triggered;

        public event EventHandler AimingEnded
        {
            add => this.m_aimingEnded += value;
            remove => this.m_aimingEnded -= value;
        }

        public event EventHandler AimingStarted
        {
            add => this.m_aimingStarted += value;
            remove => this.m_aimingStarted -= value;
        }

        private void Awake()
        {
            this.m_playerInput = new PlayerInput();
            this.m_playerInput.MainGame.Aim.started += context => this.m_aimingStarted?.Invoke(this, System.EventArgs.Empty);
            this.m_playerInput.MainGame.Aim.canceled += context => this.m_aimingEnded?.Invoke(this, System.EventArgs.Empty);
        }

        private void Update()
        {
            this.Movement = this.m_playerInput.MainGame.Move.ReadValue<Vector2>();
            this.MouseDelta = this.m_playerInput.MainGame.Mouse.ReadValue<Vector2>();
        }

        private void OnEnable()
        {
            this.m_playerInput?.Enable();
        }

        private void OnDisable()
        {
            this.m_playerInput?.Disable();
            this.Movement = Vector3.zero;
        }
    }
}