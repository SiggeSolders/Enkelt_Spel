using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Main.FinalCharacterController
{
    [DefaultExecutionOrder(-2)]
    public class PlayerMovementInput : MonoBehaviour, PlayerControlls.IPlayerMovementMapActions
    {
        public PlayerControlls PlayerControlls {  get; private set; }
        public Vector2 MovementInput {  get; private set; }
        public Vector2 LookInput { get; private set; }

        private void OnEnable()
        {
            PlayerControlls = new PlayerControlls();
            PlayerControlls.Enable();

            PlayerControlls.PlayerMovementMap.Enable();
            PlayerControlls.PlayerMovementMap.SetCallbacks(this);
        }

        private void OnDisable()
        {
            PlayerControlls.PlayerMovementMap.Disable();
            PlayerControlls.PlayerMovementMap.RemoveCallbacks(this);
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            MovementInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }
    }
}

