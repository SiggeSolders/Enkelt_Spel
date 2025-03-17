using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.FinalCharacterController
{
    [DefaultExecutionOrder(-1)]
    public class PlayerController : MonoBehaviour
    {
        #region Class Variables
        [Header("Components")]
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Camera _3rdPersonCamera;
        [SerializeField] private Camera _1stPersonCamera;

        [Header("Basic Movement")]
        public float runAcceleration = 0.25f;
        public float runSpeed = 4f;
        public float sprintAccelleration = 0.5f;
        public float sprintSpeed = 7f;
        public float drag = 0.1f;
        public float movingThreshold = 0.01f;

        [Header("Camara")]
        public float lookSenceH = 0.1f;
        public float lookSenceV = 0.1f;
        public float lookLimitV = 89f;

        private PlayerMovementInput _playerMovementInput;
        private PlayerState _playerState;

        private Vector2 _camareaRotation = Vector2.zero;
        private Vector2 _playerTargetRotation = Vector2.zero;
        #endregion

        #region Startup
        private void Awake()
        {
            _playerMovementInput = GetComponent<PlayerMovementInput>();
            _playerState = GetComponent<PlayerState>();

            _1stPersonCamera.enabled = false;
            _3rdPersonCamera.enabled = true;
        }
        #endregion

        #region Update Logic
        private void Update()
        {
            HandleLateralMovement();
            UpdateMovementState();
        }

        private void UpdateCammeraState()
        {
            
            if (_playerMovementInput.FirstPersonToggledOn == true)
            {
                _1stPersonCamera.enabled = false;
                _3rdPersonCamera.enabled = true;
            }
            else
            {
                _1stPersonCamera.enabled = true;    
                _3rdPersonCamera.enabled = false;
            }
        }

        private void UpdateMovementState()
        {
            bool isMovementInput = _playerMovementInput.MovementInput != Vector2.zero;
            bool isMovingLaterally = IsMovingLaterally();
            bool isSprinting = _playerMovementInput.SprintToggledOn && isMovingLaterally;

            PlayerMovementState lateralState =  isSprinting ? PlayerMovementState.Springting :
                                                isMovingLaterally || isMovementInput ? PlayerMovementState.Running : PlayerMovementState.Idling;
            _playerState.SetPlayerMovementState(lateralState);
        }

        private void HandleLateralMovement()
        {
            bool isSprinting = _playerState.CurrentPlayerMovementState == PlayerMovementState.Springting;

            //State dependamt Accelleration & Speed
            float lateralAccelleration = isSprinting ? sprintAccelleration : runAcceleration;
            float clampLateralMagnitue = isSprinting ? sprintSpeed : runSpeed;
            
            Vector3 cameraForwardXZ = new Vector3(_3rdPersonCamera.transform.forward.x, 0f, _3rdPersonCamera.transform.forward.z).normalized;
            Vector3 cameraRightXZ = new Vector3(_3rdPersonCamera.transform.right.x, 0f, _3rdPersonCamera.transform.right.z).normalized;
            Vector3 movementDirection = cameraRightXZ * _playerMovementInput.MovementInput.x + cameraForwardXZ * _playerMovementInput.MovementInput.y;

            Vector3 movementDelta = movementDirection * lateralAccelleration * Time.deltaTime;
            Vector3 newVelocity = _characterController.velocity + movementDelta;

            //Add drag
            Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
            newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
            newVelocity = Vector3.ClampMagnitude(newVelocity, clampLateralMagnitue);

            //Move character
            _characterController.Move(newVelocity * Time.deltaTime);
        }
        #endregion

        #region Late Update Logic
        private void LateUpdate()
        {
            _camareaRotation.x += lookSenceH * _playerMovementInput.LookInput.x;
            _camareaRotation.y = Mathf.Clamp(_camareaRotation.y - lookSenceV * _playerMovementInput.LookInput.y, -lookLimitV, lookLimitV);

            _playerTargetRotation.x += transform.eulerAngles.x + lookSenceH * _playerMovementInput.LookInput.x;
            transform.rotation = Quaternion.Euler(0f, _playerTargetRotation.x, 0f);

            _1stPersonCamera.transform.rotation = Quaternion.Euler(_camareaRotation.y, _camareaRotation.x, 0f);
        }
        #endregion

        #region State Checks
        private bool IsMovingLaterally()
        {
            Vector3 lateralVelocity = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z);

            return lateralVelocity.magnitude > movingThreshold;
        }
        #endregion
    }
}
