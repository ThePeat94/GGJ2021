using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

#if ENABLE_INPUT_SYSTEM

#endif

namespace Nidavellir.FoxIt
{
    public class SimpleCameraController : MonoBehaviour
    {
        [Header("Movement Settings")] [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
        public float boost = 3.5f;

        [Tooltip("Time it takes to interpolate camera position 99% of the way to the target.")] [Range(0.001f, 1f)]
        public float positionLerpTime = 0.2f;

        [Header("Rotation Settings")] [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
        public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

        [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target.")] [Range(0.001f, 1f)]
        public float rotationLerpTime = 0.01f;

        [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
        public bool invertY;

        private readonly CameraState m_InterpolatingCameraState = new CameraState();

        private readonly CameraState m_TargetCameraState = new CameraState();

        private void Update()
        {
            // Exit Sample  

            if (this.IsEscapePressed())
            {
                Application.Quit();
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#endif
            }

            // Hide and lock cursor when right mouse button pressed
            if (this.IsRightMouseButtonDown()) Cursor.lockState = CursorLockMode.Locked;

            // Unlock and show cursor when right mouse button released
            if (this.IsRightMouseButtonUp())
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            // Rotation
            if (this.IsCameraRotationAllowed())
            {
                var mouseMovement = this.GetInputLookRotation() * Time.deltaTime * 5;
                if (this.invertY)
                    mouseMovement.y = -mouseMovement.y;

                var mouseSensitivityFactor = this.mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

                this.m_TargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
                this.m_TargetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;
            }

            // Translation
            var translation = this.GetInputTranslationDirection() * Time.deltaTime;

            // Speed up movement when shift key held
            if (this.IsBoostPressed()) translation *= 10.0f;

            // Modify movement by a boost factor (defined in Inspector and modified in play mode through the mouse scroll wheel)
            this.boost += this.GetBoostFactor();
            translation *= Mathf.Pow(2.0f, this.boost);

            this.m_TargetCameraState.Translate(translation);

            // Framerate-independent interpolation
            // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
            var positionLerpPct = 1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / this.positionLerpTime * Time.deltaTime);
            var rotationLerpPct = 1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / this.rotationLerpTime * Time.deltaTime);
            this.m_InterpolatingCameraState.LerpTowards(this.m_TargetCameraState, positionLerpPct, rotationLerpPct);

            this.m_InterpolatingCameraState.UpdateTransform(this.transform);
        }

        private void OnEnable()
        {
            this.m_TargetCameraState.SetFromTransform(this.transform);
            this.m_InterpolatingCameraState.SetFromTransform(this.transform);
        }

        private float GetBoostFactor()
        {
#if ENABLE_INPUT_SYSTEM
            return this.boostFactorAction.ReadValue<Vector2>().y * 0.01f;
#else
            return Input.mouseScrollDelta.y * 0.2f;
#endif
        }

        private Vector2 GetInputLookRotation()
        {
#if ENABLE_INPUT_SYSTEM
            return this.lookAction.ReadValue<Vector2>();
#else
            return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 10;
#endif
        }

        private Vector3 GetInputTranslationDirection()
        {
            var direction = Vector3.zero;
#if ENABLE_INPUT_SYSTEM
            var moveDelta = this.movementAction.ReadValue<Vector2>();
            direction.x = moveDelta.x;
            direction.z = moveDelta.y;
            direction.y = this.verticalMovementAction.ReadValue<Vector2>().y;
#else
            if (Input.GetKey(KeyCode.W))
            {
                direction += Vector3.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                direction += Vector3.back;
            }
            if (Input.GetKey(KeyCode.A))
            {
                direction += Vector3.left;
            }
            if (Input.GetKey(KeyCode.D))
            {
                direction += Vector3.right;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                direction += Vector3.down;
            }
            if (Input.GetKey(KeyCode.E))
            {
                direction += Vector3.up;
            }
#endif
            return direction;
        }

        private bool IsBoostPressed()
        {
#if ENABLE_INPUT_SYSTEM
            var boost = Keyboard.current != null ? Keyboard.current.leftShiftKey.isPressed : false;
            boost |= Gamepad.current != null ? Gamepad.current.xButton.isPressed : false;
            return boost;
#else
            return Input.GetKey(KeyCode.LeftShift);
#endif
        }

        private bool IsCameraRotationAllowed()
        {
#if ENABLE_INPUT_SYSTEM
            var canRotate = Mouse.current != null ? Mouse.current.rightButton.isPressed : false;
            canRotate |= Gamepad.current != null ? Gamepad.current.rightStick.ReadValue().magnitude > 0 : false;
            return canRotate;
#else
            return Input.GetMouseButton(1);
#endif
        }

        private bool IsEscapePressed()
        {
#if ENABLE_INPUT_SYSTEM
            return Keyboard.current != null ? Keyboard.current.escapeKey.isPressed : false;
#else
            return Input.GetKey(KeyCode.Escape);
#endif
        }

        private bool IsRightMouseButtonDown()
        {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current != null ? Mouse.current.rightButton.isPressed : false;
#else
            return Input.GetMouseButtonDown(1);
#endif
        }

        private bool IsRightMouseButtonUp()
        {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current != null ? !Mouse.current.rightButton.isPressed : false;
#else
            return Input.GetMouseButtonUp(1);
#endif
        }

        private class CameraState
        {
            public float pitch;
            public float roll;
            public float x;
            public float y;
            public float yaw;
            public float z;

            public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct)
            {
                this.yaw = Mathf.Lerp(this.yaw, target.yaw, rotationLerpPct);
                this.pitch = Mathf.Lerp(this.pitch, target.pitch, rotationLerpPct);
                this.roll = Mathf.Lerp(this.roll, target.roll, rotationLerpPct);

                this.x = Mathf.Lerp(this.x, target.x, positionLerpPct);
                this.y = Mathf.Lerp(this.y, target.y, positionLerpPct);
                this.z = Mathf.Lerp(this.z, target.z, positionLerpPct);
            }

            public void SetFromTransform(Transform t)
            {
                this.pitch = t.eulerAngles.x;
                this.yaw = t.eulerAngles.y;
                this.roll = t.eulerAngles.z;
                this.x = t.position.x;
                this.y = t.position.y;
                this.z = t.position.z;
            }

            public void Translate(Vector3 translation)
            {
                var rotatedTranslation = Quaternion.Euler(this.pitch, this.yaw, this.roll) * translation;

                this.x += rotatedTranslation.x;
                this.y += rotatedTranslation.y;
                this.z += rotatedTranslation.z;
            }

            public void UpdateTransform(Transform t)
            {
                t.eulerAngles = new Vector3(this.pitch, this.yaw, this.roll);
                t.position = new Vector3(this.x, this.y, this.z);
            }
        }

#if ENABLE_INPUT_SYSTEM
        private InputAction movementAction;
        private InputAction verticalMovementAction;
        private InputAction lookAction;
        private InputAction boostFactorAction;
        private bool mouseRightButtonPressed;

        private void Start()
        {
            var map = new InputActionMap("Simple Camera Controller");

            this.lookAction = map.AddAction("look", binding: "<Mouse>/delta");
            this.movementAction = map.AddAction("move", binding: "<Gamepad>/leftStick");
            this.verticalMovementAction = map.AddAction("Vertical Movement");
            this.boostFactorAction = map.AddAction("Boost Factor", binding: "<Mouse>/scroll");

            this.lookAction.AddBinding("<Gamepad>/rightStick").WithProcessor("scaleVector2(x=15, y=15)");
            this.movementAction.AddCompositeBinding("Dpad")
                .With("Up", "<Keyboard>/w")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/s")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/a")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/d")
                .With("Right", "<Keyboard>/rightArrow");
            this.verticalMovementAction.AddCompositeBinding("Dpad")
                .With("Up", "<Keyboard>/pageUp")
                .With("Down", "<Keyboard>/pageDown")
                .With("Up", "<Keyboard>/e")
                .With("Down", "<Keyboard>/q")
                .With("Up", "<Gamepad>/rightshoulder")
                .With("Down", "<Gamepad>/leftshoulder");
            this.boostFactorAction.AddBinding("<Gamepad>/Dpad").WithProcessor("scaleVector2(x=1, y=4)");

            this.movementAction.Enable();
            this.lookAction.Enable();
            this.verticalMovementAction.Enable();
            this.boostFactorAction.Enable();
        }
#endif
    }
}