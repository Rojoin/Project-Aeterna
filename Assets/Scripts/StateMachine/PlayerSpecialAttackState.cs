using System;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace StateMachine
{
    public class PlayerSpecialAttackState : PlayerBaseState
    {
        #region References

        public AttackSO attack;
        private AttackCollision _attackCollider;
        public UnityEvent OnAttack;
        private GameSettings gameSettings;
        public event Action OnAttackEnd;

        #endregion

        public float timeBetweenCombo = 0f;
        private float lastClickedTime = 0;
        private float lastComboEnd = 0;
        private float attackRadius = 5.0f;
        private bool isAttacking;

        private float timeUntilAttackEnds;

        // private float timeUntilStart;
        private float timeAfterComboEnds;
        private float attackTimer = 0.0f;
        private event Action<Vector3> OnAttackConnected;

        private List<IHealthSystem> currentlyHitted = new List<IHealthSystem>();

        public PlayerSpecialAttackState(Action onAttackEnd, Action<Vector3> onAttackConnected, Action onMove,
            params object[] data) : base(onMove, data)
        {
            attack = data[5] as AttackSO;
            _attackCollider = data[6] as AttackCollision;
            AttackChannel = data[7] as VoidChannelSO;
            gameSettings = data[8] as GameSettings;
            OnAttackEnd += onAttackEnd;
            lastComboEnd = Time.realtimeSinceStartup;
            attackTimer = 0.0f;
            OnAttackConnected += onAttackConnected;
            currentlyHitted.Clear();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            currentlyHitted.Clear();
            //lastComboEnd = timeBetweenComboEnd;
            // AttackChannel.Subscribe(Attack);
            _attackCollider.OnTriggerEnterObject.AddListener(OnAttackEnter);
            _attackCollider.OnTriggerExitObject.AddListener(OnAttackExit);
            Attack();
        }

        public override void OnExit()
        {
            base.OnExit();

            // AttackChannel.Unsubscribe(Attack);
            _attackCollider.OnTriggerEnterObject.RemoveListener(OnAttackEnter);
            _attackCollider.OnTriggerExitObject.RemoveListener(OnAttackExit);
            currentlyHitted.Clear();
        }

        private void Attack()
        {
            if (isAttacking) return;

            float realtimeSinceStartup = Time.realtimeSinceStartup - lastComboEnd;
            if (realtimeSinceStartup * player.attackSpeed < timeBetweenCombo)

            {
                OnAttackEnd?.Invoke();
                return;
            }

            timeBetweenCombo = 0;

            Debug.Log($"Attack:");
            ActivateCollider(attack);
            _playerAnimatorController.speed = player.attackSpeed;
            _playerAnimatorController.CrossFade(attack.animationName, 0,
                0, 0);
        }

        public void ActivateCollider(AttackSO attacksParams)
        {
            StopAttack();
            _attackCollider.SetColliderParams(attacksParams.colliderCenter, attacksParams.colliderSize);

            timeUntilAttackEnds = attack.timeUntilComboEnds;
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
            // if (inputDirection != Vector2.zero)
            // {
            //     Vector3 moveDir = new Vector3(inputDirection.x, 0, inputDirection.y);
            //     float time = Time.deltaTime;
            //     rotatedMoveDir = Quaternion.AngleAxis(angle, Vector3.up) * moveDir;
            //     _characterController.Move(rotatedMoveDir * (time * player.movementSpeedDuringAttack));
            //     onMove.Invoke();
            // }
            // else
            // {
            //     rotatedMoveDir = Vector2.zero;
            // }
        }

        void EndAttackState()
        {
            StopAttack();
            lastComboEnd = Time.realtimeSinceStartup;

            timeBetweenCombo = attack.timeUntilComboEnds;

            OnAttackEnd?.Invoke();
        }

        public void StopAttack()
        {
            isAttacking = false;
            attackTimer = 0.0f;
            currentlyHitted.Clear();
            _attackCollider.ToggleCollider(false);
            _playerAnimatorController.speed = 1;
            //_playerAnimatorController.CrossFade("NormalStatus", 0.25f, 0, 0);
            //OnAttackEnd.Invoke();
        }

        private void AttackSequence(float deltaTime)
        {
            if (isAttacking)
            {
                attackTimer += deltaTime * player.attackSpeed;
                if (attackTimer >= attack.attackTime)
                {
                    EndAttackState();
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
                Vector3 direction = (other.transform.position - owner.transform.position).normalized;
                direction = new Vector3(direction.x, 0, direction.z);
                Rotate(direction);

                healthSystem.ReceiveDamage(attack.damage + player.damage);
                OnAttackConnected?.Invoke(other.transform.position);
                OnAttackEnd?.Invoke();
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            OnAttackEnd = null;
            OnAttackConnected = null;
            AttackChannel.Unsubscribe(Attack);
        }
    }
}