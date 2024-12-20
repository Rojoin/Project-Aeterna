﻿using System;
using System.Collections;
using ScriptableObjects;
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
        [SerializeField] private VoidChannelSO OnSpecialAttackChannel;
        [SerializeField] private VoidChannelSO OnInteractChannel;
        [SerializeField] private VoidChannelSO OnAlternativeInteractChannel;
        [SerializeField] private VoidChannelSO OnChangeCameraChannel;
        [SerializeField] private VoidChannelSO OnResetLevel;
        [SerializeField] private VoidChannelSO OnBackInteractChannel;
        [SerializeField] private VoidChannelSO OnHudToggleChannel;
        [SerializeField] private VoidChannelSO OnDashChannel;
        [SerializeField] private VoidChannelSO OnPauseChannel;
        [SerializeField] private VoidChannelSO OnToggleCardInformationMenuChannel;
        [SerializeField] private BoolChannelSO OnControlSchemeChange;
        [SerializeField] private GameSettings gameSettings;
        [SerializeField] private PlayerEntitySO player;
        private static bool _isGamePaused = false;

        public static bool IsGamePaused
        {
            get => _isGamePaused;
            set => _isGamePaused = value;
        }

        private bool canDash = true;
        private const int keyboardSchemeValue = 0;
        private const int gamepadSchemeValue = 1;

        private void OnEnable()
        {
            OnControlSchemeChange.RaiseEvent(false);
            OnMoveChannel.RaiseEvent(Vector2.zero);
        }

        public void OnChangeInput(PlayerInput input)
        {
            string inputCurrentControlScheme = input.currentControlScheme;
            if (inputCurrentControlScheme.Equals("Gamepad"))
            {
                OnControlSchemeChange.RaiseEvent(true);
                Debug.Log("Using Gamepad:" + inputCurrentControlScheme);
                gameSettings.isUsingController = true;
                Cursor.visible = false;
            }
            else
            {
                Cursor.visible = true;
                OnControlSchemeChange.RaiseEvent(false);
                gameSettings.isUsingController = false;
                Debug.Log("Using Mouse & Keywoard");
            }
        }

        public void OnMove(InputAction.CallbackContext ctx)
        {
            if (!IsGamePaused)
            {
                OnMoveChannel.RaiseEvent(ctx.ReadValue<Vector2>());
            }
        }

        public void OnCameraMove(InputAction.CallbackContext ctx)
        {
            OnCameraMoveChannel.RaiseEvent(ctx.ReadValue<Vector2>());
        }

        public void OnAttack(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && !IsGamePaused)
            {
                OnAttackChannel.RaiseEvent();
            }
        }

        public void OnSpecialAttack(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && !IsGamePaused)
            {
                OnSpecialAttackChannel.RaiseEvent();
            }
        }

        public void OnTargetLock(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                OnResetLevel.RaiseEvent();
            }
        }
        public void OnPause(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                OnPauseChannel.RaiseEvent();
            }
        }

        public void OnToggleCardInformationMenu(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                OnToggleCardInformationMenuChannel.RaiseEvent();
            }
        }

        public void OnChangeCamera(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && !IsGamePaused)
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
        public void OnAlternativeInteract(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                OnAlternativeInteractChannel.RaiseEvent();
            }
        }

        public void OnHud(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                OnHudToggleChannel.RaiseEvent();
            }
        }

        public void OnDash(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && canDash && !IsGamePaused)
            {
                OnDashChannel.RaiseEvent();
                canDash = false;
                StartCoroutine(DashTimer());
            }
        }

        private IEnumerator DashTimer()
        {
            yield return new WaitForSeconds(player.timebetweenDashes);
            canDash = true;
        }
    }
}