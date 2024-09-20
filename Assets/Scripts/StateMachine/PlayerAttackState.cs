using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using Unity.VisualScripting;
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

        public float timeBetweenCombo = 0.4f;
        public float timeBetweenComboEnd = 1.5f;
        private float lastClickedTime = 0;
        private float lastComboEnd = 0;
        private int comboCounter = 0;
        private float attackRadius = 5.0f;
        private bool isAttacking;
        private float timeUntilAttackEnds;
        // private float timeUntilStart;
        private float timeAfterComboEnds;
        private float attackTimer = 0.0f;

        private List<IHealthSystem> currentlyHitted = new List<IHealthSystem>();

        public PlayerAttackState(Action onAttackEnd, Action onMove, params object[] data) : base(onMove, data)
        {
            comboList = data[5] as List<AttackSO>;
            _attackCollider = data[6] as AttackCollision;
            AttackChannel = data[7] as VoidChannelSO;
            gameSettings = data[8] as GameSettings;
            OnAttackEnd += onAttackEnd;
            lastComboEnd = -timeBetweenComboEnd * 5;
            lastClickedTime = -timeBetweenCombo * 5;
            attackTimer = 0.0f;
            currentlyHitted.Clear();
            comboCounter = 0;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            currentlyHitted.Clear();
            //lastComboEnd = timeBetweenComboEnd;
            AttackChannel.Subscribe(Attack);
            Attack();
            _attackCollider.OnTriggerEnterObject.AddListener(OnAttackEnter);
            _attackCollider.OnTriggerExitObject.AddListener(OnAttackExit);
        }

        public override void OnExit()
        {
            base.OnExit();

            lastComboEnd = Time.realtimeSinceStartup;
            AttackChannel.Unsubscribe(Attack);
            _attackCollider.OnTriggerEnterObject.RemoveListener(OnAttackEnter);
            _attackCollider.OnTriggerExitObject.RemoveListener(OnAttackExit);
            currentlyHitted.Clear();
        }

        private void Attack()
        {
            if (!isAttacking)
            {
                float realtimeSinceStartup = Time.realtimeSinceStartup - lastComboEnd;
                if (realtimeSinceStartup < comboList[comboCounter].timeUntilEnd + comboList[comboCounter].attackTime)
                {
                    comboCounter++;
                    Debug.Log($"Attack:{comboCounter}");
                    if (comboCounter >= comboList.Count)
                    {
                        comboCounter = 0;
                        //lastComboEnd = Time.realtimeSinceStartup;
                    }
                }
                else
                {
                    comboCounter = 0;
                }

                Debug.Log($"Attack:{comboCounter}");

                ActivateCollider(comboList[comboCounter]);
                _playerAnimatorController.CrossFade(comboList[comboCounter].animationName, 0.25f, 0, 0);
                comboCounter++;
                CheckTarget();
                // lastClickedTime = Time.realtimeSinceStartup;
                Debug.Log($"Current clicked time is {lastClickedTime}");


                // if (Time.realtimeSinceStartup - lastComboEnd > timeBetweenComboEnd && comboCounter <= comboList.Count)
                // {
                //     Debug.Log($"The last clicked time was {lastClickedTime}");
                //     if (Time.realtimeSinceStartup - lastClickedTime >= timeBetweenCombo)
                //     {
                //         ActivateCollider(comboList[comboCounter]);
                //         _playerAnimatorController.CrossFade(comboList[comboCounter].animationName, 0.25f, 0, 0);
                //         comboCounter++;
                //         CheckTarget();
                //         lastClickedTime = Time.realtimeSinceStartup;
                //         Debug.Log($"Current clicked time is {lastClickedTime}");
                //         if (comboCounter >= comboList.Count)
                //         {
                //             comboCounter = 0;
                //             lastComboEnd = Time.realtimeSinceStartup;
                //         }
                //     }
                // }
            }
        }

        public void ActivateCollider(AttackSO attacksParams)
        {
            StopAttack();
            _attackCollider.SetColliderParams(attacksParams.colliderCenter, attacksParams.colliderSize);

            //timeUntilAttackEnds = comboList[comboCounter].attackTime + timeUntilStart;
            timeUntilAttackEnds = comboList[comboCounter].timeUntilEnd + comboList[comboCounter].timeUntilStart;
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
                if (GetRotatedMoveDir().magnitude > 0.1f)
                {
                    Rotate(GetRotatedMoveDir());
                }
                else if (GetRotatedMoveDir().magnitude <= 0.1f)
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
                    else
                    {
                        Debug.Log("No target in the area.");
                    }
                }
            }
        }

        protected override void Move(float deltaTime)
        {
            if (inputDirection != Vector2.zero)
            {
                Vector3 moveDir = new Vector3(inputDirection.x, 0, inputDirection.y);
                float time = Time.deltaTime;
                rotatedMoveDir = Quaternion.AngleAxis(angle, Vector3.up) * moveDir;
                _characterController.Move(rotatedMoveDir * (time * player.movementSpeedDuringAttack));
                onMove.Invoke();
            }
            else
            {
                rotatedMoveDir = Vector2.zero;
            }
        }

        void EndCombo()
        {
            StopAttack();
            //  lastComboEnd = Time.realtimeSinceStartup;
           // _playerAnimatorController.CrossFade("NormalStatus", 0.25f, 0, 0);
            OnAttackEnd?.Invoke();
        }

        public void StopAttack()
        {
            isAttacking = false;
            attackTimer = 0.0f;
            currentlyHitted.Clear();
            _attackCollider.ToggleCollider(false);
          //  _playerAnimatorController.CrossFade("NormalStatus", 0.25f, 0, 0);
            //OnAttackEnd.Invoke();
        }

        private void AttackSequence(float deltaTime)
        {
            if (isAttacking)
            {
                // timer += deltaTime;
                attackTimer += deltaTime;
                if (attackTimer >= comboList[comboCounter].attackTime)
                {
                    EndCombo();
                    // StopAttack();
                    Debug.Log("Finish attack");
                }
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
                Debug.Log("Exit attack.");
            }
        }

        private void OnAttackEnter(GameObject other)
        {
            if (!other.CompareTag("Player") && other.TryGetComponent<IHealthSystem>(out var healthSystem))
            {
                if (!currentlyHitted.Contains(healthSystem))
                {
                    Debug.Log("Enter attack.");
                    healthSystem.ReceiveDamage(comboList[comboCounter].damage + player.damage);
                    currentlyHitted.Add(healthSystem);
                }
                else
                {
                    Debug.Log("AttackMissed");
                }
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            OnAttackEnd = null;
            //    AttackChannel.Unsubscribe(Attack);
        }
    }
}