using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Cinemachine;
using EventArgs;
using Scriptables;
using UI;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private static PlayerController s_instance;

    [SerializeField] private PlayerData m_playerData;
    [SerializeField] private GameObject m_aimCameraGameObject;
    [SerializeField] private CinemachineFreeLook m_freeLookCamera;
    [SerializeField] private AimCamera m_aimCamera;
    [SerializeField] private GameObject m_arrowPrefab;
    [SerializeField] private GameObject m_spawnPoint;
    [SerializeField] private GameObject m_crossHair;
    [SerializeField] private PlayerHud m_playerHud;

    private Vector3 m_moveDirection;
    private CharacterController m_characterController;
    private InputProcessor m_inputProcessor;
    private Animator m_animator;
    private Transform m_camera;

    private Coroutine m_reloadCoroutine;

    private float m_currentShootcooldown;

    private bool m_isAiming;
    private bool m_isDead;

    private static readonly int s_isIdleHash = Animator.StringToHash("IsIdle");
    private static readonly int s_isRunningHash = Animator.StringToHash("IsRunning");
    private static readonly int s_isAimingHash = Animator.StringToHash("IsAiming");
    private static readonly int s_dirXHash = Animator.StringToHash("DirX");
    private static readonly int s_dirZHash = Animator.StringToHash("DirZ");

    public static PlayerController Instance => s_instance;
    public ResourceController HealthController { get; set; }
    public ResourceController QuiverController { get; set; }

    public float MovementSpeed
    {
        get;
        set;
    }

    public float ReloadTime
    {
        get;
        set;
    }
    
    public int AttackDamage
    {
        get;
        set;
    }
    
    
    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        
        this.HealthController = new ResourceController(this.m_playerData.HealthData);
        this.QuiverController = new ResourceController(this.m_playerData.QuiverData);
        this.m_inputProcessor = this.GetComponent<InputProcessor>();
        this.m_characterController = this.GetComponent<CharacterController>();
        this.m_animator = this.GetComponent<Animator>();
        this.m_camera = Camera.main.transform;
        this.m_inputProcessor.AimingStarted += (sender, args) => this.StartAiming();
        this.m_inputProcessor.AimingEnded += (sender, args) => this.StopAiming();
        this.HealthController.ResourceValueChanged += HealthControllerOnResourceValueChanged;
        this.MovementSpeed = this.m_playerData.MovementSpeed;
        this.ReloadTime = this.m_playerData.ReloadingDuration;
        this.AttackDamage = this.m_playerData.AttackDamage;

    }

    private void HealthControllerOnResourceValueChanged(object sender, ResourceValueChangedEventArgs e)
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

    // Update is called once per frame
    void Update()
    {
        if (this.m_isDead)
        {
            if (this.m_inputProcessor.ReloadTriggered)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            else if (this.m_inputProcessor.QuitTriggered)
                Application.Quit();
            
            return;
        }


        if(this.m_currentShootcooldown > 0f)
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
            if (this.m_inputProcessor.ShootTriggered && this.m_reloadCoroutine == null && this.m_currentShootcooldown <= 0f)
            {
                this.Shoot();
            }
        }

        if (this.m_inputProcessor.ReloadTriggered && this.m_reloadCoroutine == null)
        {
            this.m_reloadCoroutine = StartCoroutine(this.Reload());
        }
    }

    private void LateUpdate()
    {
        this.UpdateAnimator();
    }
    
    private void Move()
    {
        this.m_moveDirection = this.m_camera.forward * this.m_inputProcessor.Movement.y;
        this.m_moveDirection += this.m_camera.right * this.m_inputProcessor.Movement.x;
        this.m_moveDirection.y = 0;
        this.m_characterController.SimpleMove(this.m_moveDirection  * this.MovementSpeed);
    }

    private void MoveSidewards()
    {
        this.m_moveDirection = this.transform.forward * this.m_inputProcessor.Movement.y;
        this.m_moveDirection += this.transform.right * this.m_inputProcessor.Movement.x;
        this.m_moveDirection.y = 0;
        this.m_characterController.SimpleMove(this.m_moveDirection * this.m_playerData.AimingMovementSpeed);
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

    private void AimRotate()
    {
        this.transform.Rotate(Vector3.up, this.m_inputProcessor.MouseDelta.x * this.m_playerData.MouseSensivity);
    }

    private void UpdateAnimator()
    {
        this.m_animator.SetBool(s_isAimingHash, this.m_isAiming);
        this.m_animator.SetFloat(s_dirXHash, this.m_inputProcessor.Movement.x);
        this.m_animator.SetFloat(s_dirZHash, this.m_inputProcessor.Movement.y);
        this.m_animator.SetBool(s_isRunningHash, !this.m_isAiming && this.m_inputProcessor.Movement != Vector2.zero);
        this.m_animator.SetBool(s_isIdleHash, !this.m_isAiming && this.m_inputProcessor.Movement == Vector2.zero);
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

    private void Shoot()
    {
        if(!this.QuiverController.CanAfford(1))
            return;

        this.m_currentShootcooldown = this.m_playerData.ShootCooldown;
        this.QuiverController.UseResource(1);
        
        Vector3 shootdirection = Vector3.zero;
        if (!this.m_isAiming)
        {
            shootdirection = this.transform.forward;
        }
        else
        {
            float x = Screen.width / 2f;
            float y = Screen.height / 2f;

            var ray = Camera.main.ScreenPointToRay(new Vector3(x, y, 0));

            if (Physics.Raycast(this.m_camera.position, this.m_camera.forward, out var hit))
            {
                shootdirection = hit.point - this.m_spawnPoint.transform.position;
            }
            else
            {
                shootdirection = ray.direction;
            }
        }
        
        var instantiatedArrow = Instantiate(this.m_arrowPrefab, this.m_spawnPoint.transform.position, Quaternion.identity).GetComponent<Arrow>();
        instantiatedArrow.Damage = this.AttackDamage;
        var rig = instantiatedArrow.GetComponent<Rigidbody>();
        rig.AddForce(shootdirection.normalized * this.m_playerData.ShootForce, ForceMode.Impulse);
        CameraSoundPlayer.Instance.PlayClipAtCam(this.m_playerData.ArrowShootSound, 1f);
    }

    private IEnumerator Reload()
    {
        this.m_playerHud.ShowReloadUI(this.ReloadTime);
        CameraSoundPlayer.Instance.PlayClipAtCam(this.m_playerData.ReloadSound);
        yield return new WaitForSeconds(this.ReloadTime);
        this.QuiverController.ResetValue();
        this.m_reloadCoroutine = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        var minifox = other.GetComponent<Minifox>();
        if (minifox != null)
        {
            minifox.Upgrade.ApplyUpgrade();
            minifox.ShowDialogue();
            Destroy(minifox.gameObject);
            CameraSoundPlayer.Instance.PlayClipAtCam(this.m_playerData.CollectFoxSound);
            return;
        }
    }
}
