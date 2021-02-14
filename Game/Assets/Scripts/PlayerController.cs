using System.Collections;
using System.Linq;
using Cinemachine;
using Nidavellir.FoxIt.EventArgs;
using Nidavellir.FoxIt.Interfaces;
using Nidavellir.FoxIt.Scriptables;
using Nidavellir.FoxIt.UI;
using Nidavellir.FoxIt.Utils;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nidavellir.FoxIt
{
    public class PlayerController : MonoBehaviour
    {
        private static readonly int s_isIdleHash = Animator.StringToHash("IsIdle");
        private static readonly int s_isRunningHash = Animator.StringToHash("IsRunning");
        private static readonly int s_isAimingHash = Animator.StringToHash("IsAiming");
        private static readonly int s_dirXHash = Animator.StringToHash("DirX");
        private static readonly int s_dirZHash = Animator.StringToHash("DirZ");

        [SerializeField] private PlayerData m_playerData;
        [SerializeField] private GameObject m_aimCameraGameObject;
        [SerializeField] private CinemachineFreeLook m_freeLookCamera;
        [SerializeField] private AimCamera m_aimCamera;
        [SerializeField] private GameObject m_arrowPrefab;
        [SerializeField] private GameObject m_spawnPoint;
        [SerializeField] private GameObject m_crossHair;
        [SerializeField] private PlayerHud m_playerHud;
        [SerializeField] private Transform m_cameraLookForward;
        [SerializeField] private DialogueManager m_dialogueManager;
        private Animator m_animator;
        private Transform m_camera;
        private CharacterController m_characterController;
        private float m_currentShootcooldown;
        private GameInputProcessor m_gameInputProcessor;
        private bool m_isAiming;
        private bool m_isDead;
        private Vector3 m_moveDirection;
        private Coroutine m_reloadCoroutine;
        
        private ITalkable m_currentTalkable;

        public static PlayerController Instance { get; private set; }
        
        public ResourceController HealthController { get; set; }
        public ResourceController QuiverController { get; set; }
        public float MovementSpeed { get; set; }
        public float ReloadTime { get; set; }
        public int AttackDamage { get; set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }
            
            this.HealthController = new ResourceController(this.m_playerData.HealthData);
            this.QuiverController = new ResourceController(this.m_playerData.QuiverData);
            this.m_gameInputProcessor = this.GetComponent<GameInputProcessor>();
            this.m_characterController = this.GetComponent<CharacterController>();
            this.m_animator = this.GetComponent<Animator>();
            this.m_camera = Camera.main.transform;
            this.m_gameInputProcessor.AimingStarted += (sender, args) => this.StartAiming();
            this.m_gameInputProcessor.AimingEnded += (sender, args) => this.StopAiming();
            this.HealthController.ResourceValueChanged += this.HealthControllerOnResourceValueChanged;
            this.MovementSpeed = this.m_playerData.MovementSpeed;
            this.ReloadTime = this.m_playerData.ReloadingDuration;
            this.AttackDamage = this.m_playerData.AttackDamage;
            
            this.m_dialogueManager.DialogueStarted += this.DialogueStarted;
            this.m_dialogueManager.DialogueEnded += this.DialogueEnded;
        }

        private void DialogueEnded(object sender, System.EventArgs e)
        {
            this.m_gameInputProcessor.enabled = true;
        }

        private void DialogueStarted(object sender, System.EventArgs e)
        {
            this.m_gameInputProcessor.enabled = false;
        }

        // Update is called once per frame
        private void Update()
        {
            if (this.m_isDead)
            {
                if (this.m_gameInputProcessor.ReloadTriggered)
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                else if (this.m_gameInputProcessor.QuitTriggered)
                    Application.Quit();

                return;
            }

            if (this.m_currentShootcooldown > 0f)
                this.m_currentShootcooldown -= Time.deltaTime;

            if (!this.m_isAiming)
            {
                this.Move();
                this.Rotate();
            }
            else
            {
                this.MoveSidewards();
                this.AimRotate();
                if (this.m_gameInputProcessor.ShootTriggered && this.m_reloadCoroutine == null && this.m_currentShootcooldown <= 0f) 
                    this.Shoot();
            }

            var canReload = this.QuiverController.CurrentValue < this.QuiverController.MaxValue && this.m_gameInputProcessor.ReloadTriggered && this.m_reloadCoroutine == null;
            if (canReload)
                this.m_reloadCoroutine = this.StartCoroutine(this.Reload());
        }

        private void LateUpdate()
        {
            this.UpdateAnimator();
        }
        
        private void AimRotate()
        {
            this.transform.Rotate(Vector3.up, this.m_gameInputProcessor.MouseDelta.x * this.m_playerData.MouseSensivity);
        }

        private void HealthControllerOnResourceValueChanged(object sender, ResourceValueChangedEvent e)
        {
            if (this.m_isDead)
                return;

            if (e.NewValue <= 0f)
            {
                this.m_isDead = true;
                this.StopAiming();
                this.m_animator.SetTrigger("Die");
                MusicPlayer.Instance.PlayInstant(this.m_playerData.GameOverTheme, false);
            }
        }

        private void Move()
        {
            this.m_moveDirection = this.m_cameraLookForward.forward * this.m_gameInputProcessor.Movement.y;
            this.m_moveDirection += this.m_cameraLookForward.right * this.m_gameInputProcessor.Movement.x;
            this.m_moveDirection.y = 0;
            
            if (!this.m_characterController.isGrounded)
                this.m_moveDirection.y = Physics.gravity.y * 100f;

            this.m_characterController.Move(this.m_moveDirection * (this.MovementSpeed * Time.deltaTime));
        }

        private void MoveSidewards()
        {
            this.m_moveDirection = this.transform.forward * this.m_gameInputProcessor.Movement.y;
            this.m_moveDirection += this.transform.right * this.m_gameInputProcessor.Movement.x;
            this.m_moveDirection.y = 0;
            this.m_characterController.SimpleMove(this.m_moveDirection * this.m_playerData.AimingMovementSpeed);
        }

        private IEnumerator Reload()
        {
            this.m_playerHud.ShowReloadUI(this.ReloadTime);
            CameraSoundPlayer.Instance.PlayClipAtCam(this.m_playerData.ReloadSound, 1f);
            yield return new WaitForSeconds(this.ReloadTime);
            this.QuiverController.ResetValue();
            this.m_reloadCoroutine = null;
        }

        private void Rotate()
        {
            var targetDir = this.m_moveDirection;
            targetDir.y = 0f;

            if (targetDir == Vector3.zero)
                targetDir = this.transform.forward;

            this.RotateTowards(targetDir);
        }

        private void RotateTowards(Vector3 dir)
        {
            var lookRotation = Quaternion.LookRotation(dir.normalized);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookRotation, this.m_playerData.RotationSpeed * Time.deltaTime);
        }

        private void Shoot()
        {
            if (!this.QuiverController.CanAfford(1))
                return;

            this.m_currentShootcooldown = this.m_playerData.ShootCooldown;
            this.QuiverController.UseResource(1);

            Vector3 shootDirection;
            if (!this.m_isAiming)
            {
                shootDirection = this.transform.forward;
            }
            else
            {
                var x = Screen.width / 2f;
                var y = Screen.height / 2f;

                var ray = Camera.main.ScreenPointToRay(new Vector3(x, y, 0));

                if (Physics.Raycast(this.m_camera.position, this.m_camera.forward, out var hit))
                    shootDirection = hit.point - this.m_spawnPoint.transform.position;
                else
                    shootDirection = ray.direction;
            }

            var instantiatedArrow = Instantiate(this.m_arrowPrefab, this.m_spawnPoint.transform.position, Quaternion.identity).GetComponent<Arrow>();
            instantiatedArrow.Damage = this.AttackDamage;
            instantiatedArrow.ShootAt(shootDirection, this.m_playerData.ShootForce);
            CameraSoundPlayer.Instance.PlayClipAtCam(this.m_playerData.ArrowShootSound, 1f);
        }

        private void StartAiming()
        {
            var aimDirection = this.m_camera.forward;
            aimDirection.y = 0f;
            this.transform.rotation = Quaternion.LookRotation(aimDirection);
            this.m_isAiming = true;
            this.m_aimCamera.enabled = true;
            this.m_freeLookCamera.enabled = false;
            this.m_camera.SetParent(this.m_aimCameraGameObject.transform);
            this.m_camera.localRotation = Quaternion.identity;
            this.m_camera.localPosition = Vector3.zero;
            this.m_crossHair.SetActive(true);
        }

        private void StopAiming()
        {
            this.m_isAiming = false;
            this.m_freeLookCamera.enabled = true;
            this.m_aimCamera.enabled = false;
            this.m_camera.SetParent(null);
            this.m_crossHair.SetActive(false);
        }

        private void UpdateAnimator()
        {
            this.m_animator.SetBool(s_isAimingHash, this.m_isAiming);
            this.m_animator.SetFloat(s_dirXHash, this.m_gameInputProcessor.Movement.x);
            this.m_animator.SetFloat(s_dirZHash, this.m_gameInputProcessor.Movement.y);
            this.m_animator.SetBool(s_isRunningHash, !this.m_isAiming && this.m_gameInputProcessor.Movement != Vector2.zero);
            this.m_animator.SetBool(s_isIdleHash, !this.m_isAiming && this.m_gameInputProcessor.Movement == Vector2.zero);
        }
    }
}