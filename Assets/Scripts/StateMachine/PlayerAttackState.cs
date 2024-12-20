﻿using System;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace StateMachine
{
    public class PlayerAttackState : PlayerBaseState
    {
        #region References

        public List<AttackSO> comboList;
        private AttackCollision _attackCollider;
        public UnityEvent OnAttack;
        private GameSettings gameSettings;
        public event Action OnAttackEnd;

        #endregion

        public float timeBetweenCombo = 0f;
        private float lastClickedTime = 0;
        private float lastComboEnd = 0;
        private int comboCounter = 0;
        private float attackRadius = 5.0f;
        private bool isAttacking;
        private Coroutine AttackingCorroutine;

        private float timeUntilAttackEnds;
        private Vector3 currentInputDirection;

        // private float timeUntilStart;
        private float timeAfterComboEnds;
        private float attackTimer = 0.0f;
        private bool isInputBuffered = false;

        private List<IHealthSystem> currentlyHitted = new List<IHealthSystem>();

        public PlayerAttackState(Action onAttackEnd, Action onMove, params object[] data) : base(onMove, data)
        {
            comboList = data[5] as List<AttackSO>;
            _attackCollider = data[6] as AttackCollision;
            AttackChannel = data[7] as VoidChannelSO;
            gameSettings = data[8] as GameSettings;
            OnAttackEnd += onAttackEnd;
            lastComboEnd = Time.realtimeSinceStartup;
            attackTimer = 0.0f;
            currentlyHitted.Clear();
            comboCounter = comboList.Count - 1;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            AttackChannel.Subscribe(Attack);
            _attackCollider.OnTriggerEnterObject.AddListener(OnAttackEnter);
            _attackCollider.OnTriggerExitObject.AddListener(OnAttackExit);
         
            StartAttack();
        }

        private void StartAttack()
        {  
            currentlyHitted.Clear();
            isInputBuffered = false;
            Attack();
            if (inputDirection != Vector2.zero)
            {
                currentInputDirection = new Vector3(inputDirection.x, 0, inputDirection.y);
                Vector3 moveDir = new Vector3(inputDirection.x, 0, inputDirection.y);
                rotatedMoveDir = Quaternion.AngleAxis(angle, Vector3.up) * moveDir;
                if (gameSettings.isUsingController)
                {
                    Rotate(rotatedMoveDir);
                }
            }
            else
            {
                currentInputDirection = Vector3.zero;
            }
        }


        public override void OnExit()
        {
            base.OnExit();

            currentInputDirection = Vector3.zero;
            AttackChannel.Unsubscribe(Attack);
            _attackCollider.OnTriggerEnterObject.RemoveListener(OnAttackEnter);
            _attackCollider.OnTriggerExitObject.RemoveListener(OnAttackExit);
            currentlyHitted.Clear();
        }

        private void Attack()
        {
            if (isAttacking)
            {
                isInputBuffered = true;
                return;
            }

            float realtimeSinceStartup = Time.realtimeSinceStartup - lastComboEnd;
            if (realtimeSinceStartup * player.attackSpeed < timeBetweenCombo)
            {
                OnAttackEnd?.Invoke();
                return;
            }

            timeBetweenCombo = 0;

            if (realtimeSinceStartup < timeUntilAttackEnds)
            {
                comboCounter++;
                if (comboCounter >= comboList.Count)
                {
                    comboCounter = 0;
                }
            }
            else
            {
                comboCounter = 0;
            }
            

            SetAttackParametters(comboList[comboCounter]);
            _playerAnimatorController.speed = player.attackSpeed;
            _playerAnimatorController.CrossFade(comboList[comboCounter].animationName, comboCounter > 0 ? 0.25f : 0,
                0, 0);

            CheckTarget();
            // lastClickedTime = Time.realtimeSinceStartup;
        }

        public void SetAttackParametters(AttackSO attacksParams)
        {
            ResetAttackParameters();
            timeUntilAttackEnds = attacksParams.timeUntilComboEnds;
            isAttacking = true;
        }

        private void CheckTarget()
        {
            if (!gameSettings.isUsingController)
            {
                Vector3 characterPosition = owner.transform.position;
                Vector3 mouseScreenPosition = Input.mousePosition;

                Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
                Plane plane = new Plane(Vector3.up, characterPosition);
                if (plane.Raycast(ray, out float distance))
                {
                    Vector3 mouseWorldPosition = ray.GetPoint(distance);
                    Vector3 direction = mouseWorldPosition - characterPosition;
                    direction.y = 0;
                    Rotate(direction);
                }
            }

            else if (gameSettings.isUsingController)
            {
                float minDirMagnitude = 0.1f;
                if (GetRotatedMoveDir().magnitude > minDirMagnitude)
                {
                    Rotate(GetRotatedMoveDir());
                }
                else if (GetRotatedMoveDir().magnitude < minDirMagnitude)
                {
                    Collider[] possibleTargets =
                        Physics.OverlapSphere(owner.transform.position, attackRadius, LayerMask.GetMask($"Target"));
                    Vector3 direction;
                    if (possibleTargets.Length > 0)
                    {
                        GameObject currentTarget = null;
                        float minDistance = 999;
                        currentTarget = possibleTargets[0].gameObject;

                        foreach (Collider target in possibleTargets)
                        {
                            float distanceToTarget =
                                Vector3.Distance(target.transform.position, owner.transform.position);
                            if (distanceToTarget < minDistance)
                            {
                                currentTarget = target.gameObject;
                            }
                        }

                        direction = (currentTarget.transform.position - owner.transform.position).normalized;
                        direction = new Vector3(direction.x, 0, direction.z);
                        Rotate(direction);
                    }
                }
            }
        }

        protected override void Move(float deltaTime)
        {
            if (inputDirection != Vector2.zero)
            {
                Vector3 moveDir = new Vector3(inputDirection.x, 0, inputDirection.y);
                rotatedMoveDir = Quaternion.AngleAxis(angle, Vector3.up) * moveDir;
            }
            else
            {
                rotatedMoveDir = Vector2.zero;
            }
        }

        void EndAttackState()
        {
            ResetAttackParameters();
            lastComboEnd = Time.realtimeSinceStartup;

            if (comboCounter >= comboList.Count - 1)
            {
                timeBetweenCombo = comboList[comboCounter].timeUntilComboEnds;
                OnAttackEnd?.Invoke();
            }
            else if (isInputBuffered)
            {
                isInputBuffered = false;
                isAttacking = false;
                attackTimer = 0.0f;
                StartAttack();
            }
            else
            {
                OnAttackEnd?.Invoke();
            }

            // _playerAnimatorController.CrossFade("NormalStatus", 0.25f, 0, 0);
        
        }

        public void ResetAttackParameters()
        {
            isAttacking = false;
            attackTimer = 0.0f;
            currentlyHitted.Clear();
            _attackCollider.ToggleCollider(false);
            _playerAnimatorController.speed = 1;

            // currentInputDirection = Vector3.zero;

            //  _playerAnimatorController.CrossFade("NormalStatus", 0.25f, 0, 0);
            //OnAttackEnd.Invoke();
        }

        private void AttackSequence(float deltaTime)
        {
            if (isAttacking)
            {
                attackTimer += deltaTime * player.attackSpeed;
                if (attackTimer >= comboList[comboCounter].attackTime)
                {
                    EndAttackState();
                    // StopAttack();
                }

                float normalizedTime = (attackTimer / comboList[comboCounter].attackTime);
                float impulse = comboList[comboCounter].animationCurve.Evaluate(normalizedTime);
                if (comboList[comboCounter].attackMovementType == AttackMovementTypes.InputDirection)
                {
                    rotatedMoveDir = Quaternion.AngleAxis(angle, Vector3.up) * currentInputDirection;
                    _characterController.Move(rotatedMoveDir * (deltaTime * player.speed * impulse));
                }
                else
                {
                    _characterController.Move(owner.transform.forward * (deltaTime * player.speed * impulse));
                }

                onMove.Invoke();
            }
        }

        public override void OnTick(params object[] data)
        {
            base.OnTick(data);
            float deltaTime = (float)data[0];
            AttackSequence(deltaTime);
        }

        private void OnAttackExit(GameObject other)
        {
            if (!other.CompareTag("Player") && other.TryGetComponent<IHealthSystem>(out var healthSystem))
            {
   
            }
        }

        private void OnAttackEnter(GameObject other)
        {
            if (!other.CompareTag("Player") && other.TryGetComponent<IHealthSystem>(out var healthSystem))
            {
                if (!currentlyHitted.Contains(healthSystem))
                {
       
                    if (player.hasReverseTheStars && comboCounter == comboList.Count - 1)
                    {
                        healthSystem.ReceiveDamage(
                            comboList[comboCounter].damage + player.damage + player.theStarDamage);
                    }
                    else
                    {
                        healthSystem.ReceiveDamage(
                            comboList[comboCounter].damage + player.damage);
                    }
                    owner.StartRumble(player.rumbleHittingEnemyDuration,player.rumbleHittingEnemyForce);
                    if (other.TryGetComponent<IMovevable>(out var movevable) &&
                        comboList[comboCounter].attackMovementType == AttackMovementTypes.Forward)
                    {
                        movevable.Move(owner.transform.forward, player.speed, comboList[comboCounter].attackTime,
                            comboList[comboCounter].animationCurve);
                    }

                    currentlyHitted.Add(healthSystem);
                }
 
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            OnAttackEnd = null;
            AttackChannel.Unsubscribe(Attack);
        }
    }
}