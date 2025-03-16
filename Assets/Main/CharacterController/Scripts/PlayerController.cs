using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.FinalCharacterController
{
    [DefaultExecutionOrder(-1)]
    public class PlayerController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Camera _playerCamera;

        [Header("Basic Movement")]
        public float runAcceleration = 0.25f;
        public float runSpeed = 4f;
        public float drag = 0.1f;

        [Header("Camara")]
        public float lookSenceH = 0.1f;
        public float lookSenceV = 0.1f;
        public float lookLimitV = 89f;

        private PlayerMovementInput _playerMovementInput;
        private Vector2 _camareaRotation = Vector2.zero;
        private Vector2 _playerTargetRotation = Vector2.zero;

        private void Awake()
        {
            _playerMovementInput = GetComponent<PlayerMovementInput>();

        }
        private void Update()
        {
            Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
            Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;
            Vector3 movementDirection = cameraRightXZ * _playerMovementInput.MovementInput.x + cameraForwardXZ * _playerMovementInput.MovementInput.y;

            Vector3 movementDelta = movementDirection * runAcceleration *Time.deltaTime;
            Vector3 newVelocity = _characterController.velocity + movementDelta;

            //Add drag
            Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
            newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
            newVelocity = Vector3.ClampMagnitude(newVelocity, runSpeed);

            //Move character
            _characterController.Move(newVelocity * Time.deltaTime);
        }

        private void LateUpdate()
        {
            _camareaRotation.x += lookSenceH * _playerMovementInput.LookInput.x;
            _camareaRotation.y = Mathf.Clamp(_camareaRotation.y - lookSenceV * _playerMovementInput.LookInput.y, -lookLimitV, lookLimitV);

            _playerTargetRotation.x += transform.eulerAngles.x + lookSenceH * _playerMovementInput.LookInput.x;
            transform.rotation = Quaternion.Euler(0f, _playerTargetRotation.x, 0f);

            _playerCamera.transform.rotation = Quaternion.Euler(_camareaRotation.y, _camareaRotation.x, 0f);
        }
    }
}
