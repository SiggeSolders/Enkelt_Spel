using System;
using UnityEngine;

namespace Main.FinalCharacterController
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private float movementBlendSpeed = 0.02f;

        private PlayerMovementInput _playerMovementInput;
        private PlayerState _playerState;

        private static int inputXHash = Animator.StringToHash("InputX");
        private static int inputYHash = Animator.StringToHash("InputY");
        private static int inputMagnitudeHash = Animator.StringToHash("InputMagnitude");

        private Vector3 _currentBlendInput = Vector3.zero;

        private void Awake()
        {
            _playerMovementInput = GetComponent<PlayerMovementInput>();
            _playerState = GetComponent<PlayerState>();
        }

        private void Update()
        {
            UpdateAnimationState();
        }

        private void UpdateAnimationState()
        {
            bool isSprinting = _playerState.CurrentPlayerMovementState == PlayerMovementState.Springting;

            Vector2 inputTarget = isSprinting ? _playerMovementInput.MovementInput * 1.5f : _playerMovementInput.MovementInput;
            _currentBlendInput = Vector3.Lerp(_currentBlendInput, inputTarget, movementBlendSpeed * Time.deltaTime);

            _animator.SetFloat(inputXHash, _currentBlendInput.x);
            _animator.SetFloat(inputYHash, _currentBlendInput.y);
            _animator.SetFloat(inputMagnitudeHash, _currentBlendInput.magnitude);
        }
    }
}
