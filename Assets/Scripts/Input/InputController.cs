﻿using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace InputControls
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private Vector2ChannelSO OnMoveChannel;
        [SerializeField] private Vector2ChannelSO OnCameraMoveChannel;
        [SerializeField] private VoidChannelSO OnAttackChannel;
        [SerializeField] private VoidChannelSO OnInteractChannel;
        [SerializeField] private VoidChannelSO OnChangeCameraChannel;
     [SerializeField] private VoidChannelSO OnResetLevel;
        [SerializeField] private VoidChannelSO OnBackInteractChannel;
        [SerializeField] private IntChannelSO OnControlSchemeChange;
        private const int keyboardSchemeValue = 0;
        private const int gamepadSchemeValue = 1;

        private void OnEnable()
        {
            
        }

        public void OnChangeInput(PlayerInput input)
        {
            string inputCurrentControlScheme = input.currentControlScheme;
            if (inputCurrentControlScheme.Equals("Gamepad"))
            {
                OnControlSchemeChange.RaiseEvent(gamepadSchemeValue);
                Debug.Log("Using Gamepad:" + inputCurrentControlScheme);
               // _playerStats.ChangeControllerInput(gamepadSchemeValue);
            }
            else
            {
                OnControlSchemeChange.RaiseEvent(keyboardSchemeValue);
              //  _playerStats.ChangeControllerInput(keyboardSchemeValue);
                Debug.Log("Using Mouse & Keywoard");
            }
        }
        
        public void OnMove(InputAction.CallbackContext ctx)
        {
            OnMoveChannel.RaiseEvent(ctx.ReadValue<Vector2>());
        }  public void OnCameraMove(InputAction.CallbackContext ctx)
        {
            OnCameraMoveChannel.RaiseEvent(ctx.ReadValue<Vector2>());
        }
        
        public void OnAttack(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                OnAttackChannel.RaiseEvent();
            }
        }
        public void OnTargetLock(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                OnResetLevel.RaiseEvent();
            }
        }
        public void OnChangeCamera(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                OnChangeCameraChannel.RaiseEvent();
            }
        } 
        public void OnInteract(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                OnInteractChannel.RaiseEvent();
            }
        }

    }
}