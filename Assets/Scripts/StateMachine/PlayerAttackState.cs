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
        public event Action OnAttackEnd;

        #endregion

        public float timeBetweenCombo = 1.0f;
        public float timeBetweenComboEnd = 0.5f;
        private float lastClickedTime = 0;
        private float lastComboEnd = 0;
        private int comboCounter = 0;
        private float attackTimer;
        private float attackRadius = 5.0f;
        public bool isMouseActive;
        private GameObject currentTarget;
        private Coroutine _attacking;
        private float timeUntilAttackEnds;
        private float timeUntilStart;


        public PlayerAttackState(Action onAttackEnd, params object[] data) : base(data)
        {
            comboList = data[6] as List<AttackSO>;
            _attackCollider = data[7] as AttackCollision;
            AttackChannel = data[8] as VoidChannelSO;
            OnAttackEnd += onAttackEnd;
            isMouseActive = true;
            lastComboEnd = timeBetweenComboEnd;
            lastClickedTime = -timeBetweenCombo;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            lastComboEnd = timeBetweenComboEnd;
            AttackChannel.Subscribe(Attack);
            Attack();
            _attackCollider.OnTriggerEnterObject.AddListener(OnAttackEnter);
            _attackCollider.OnTriggerExitObject.AddListener(OnAttackExit);
        }

        public override void OnExit()
        {
            base.OnExit();
            // StopAttack();
            // comboCounter = 0;
            // lastComboEnd = Time.time;
            // _playerAnimatorController.CrossFade("NormalStatus", 0.25f, 0, 0);
            lastComboEnd = Time.time;
            AttackChannel.Unsubscribe(Attack);
            _attackCollider.OnTriggerEnterObject.RemoveListener(OnAttackEnter);
            _attackCollider.OnTriggerExitObject.RemoveListener(OnAttackExit);
        }

        private void Attack()
        {
            if (Time.time - lastComboEnd > timeBetweenComboEnd && comboCounter <= comboList.Count)
            {
                Debug.Log($"The last clicked time was {lastClickedTime}");
                if (Time.time - lastClickedTime >= timeBetweenCombo)
                {
                    ActivateCollider(comboList[comboCounter]);
                    _playerAnimatorController.CrossFade(comboList[comboCounter].animationName, 0.25f, 0, 0);
                    comboCounter++;
                    if (GetRotatedMoveDir().magnitude <= 0.1f)
                    {
                        CheckTarget();
                    }

                    lastClickedTime = Time.time;
                    Debug.Log($"Current clicked time is {lastClickedTime}");
                    if (comboCounter >= comboList.Count)
                    {
                        comboCounter = 0;
                    }
                }
            }
        }

        public void ActivateCollider(AttackSO attacksParams)
        {
            StopAttack();
            _attackCollider.SetColliderParams(attacksParams.colliderCenter, attacksParams.colliderSize);
            timeUntilStart = attacksParams.timeUntilStart;
            timeUntilAttackEnds = attacksParams.attackTime - timeUntilStart;

            _attacking = owner.GetComponent<MonoBehaviour>().StartCoroutine(AttackCorroutine());
        }

        private void CheckTarget()
        {
            if (isMouseActive)
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
            else
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
                        float distanceToTarget = Vector3.Distance(target.transform.position, owner.transform.position);
                        if (distanceToTarget < minDistance)
                        {
                            currentTarget = target.gameObject;
                        }

                        direction = (currentTarget.transform.position - owner.transform.position).normalized;
                        direction = new Vector3(direction.x, 0, direction.z);
                        Rotate(direction);
                    }
                }
                else
                {
                    Debug.Log("No target in the area.");
                }
            }
        }

        void EndCombo()
        {
            StopAttack();
            comboCounter = 0;
            lastComboEnd = Time.time;
            _playerAnimatorController.CrossFade("NormalStatus", 0.25f, 0, 0);
            OnAttackEnd?.Invoke();
        }

        public void StopAttack()
        {
            if (_attacking != null)
            {
                owner.GetComponent<MonoBehaviour>().StopCoroutine(_attacking);
            }

            _attackCollider.ToggleCollider(false);
        }

        private IEnumerator AttackCorroutine()
        {
            timeUntilStart = comboList[comboCounter].timeUntilStart;
            var timeAfterComboEnds = comboList[comboCounter].timeUntilEnd;
            timeUntilAttackEnds = comboList[comboCounter].attackTime - timeUntilStart - timeAfterComboEnds;
            yield return new WaitForSeconds(timeUntilStart);
            // OnAttack.Invoke();
            _attackCollider.ToggleCollider(true);
            yield return new WaitForSeconds(timeUntilAttackEnds);
            _attackCollider.ToggleCollider(false);
            yield return new WaitForSeconds(timeAfterComboEnds);
            EndCombo();
            yield break;
        }

        public override IEnumerator Movement(Vector2 dir)
        {
            while (dir != Vector2.zero)
            {
                Vector3 moveDir = new Vector3(dir.x, 0, dir.y);
                float time = Time.deltaTime;
                rotatedMoveDir = Quaternion.AngleAxis(-45, Vector3.up) * moveDir;
                _characterController.Move(rotatedMoveDir * (time * player.speed));
                yield return null;
            }

            rotatedMoveDir = Vector2.zero;
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
                Debug.Log("Enter attack.");
                healthSystem.ReceiveDamage(comboList[comboCounter].damage);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            OnAttackEnd = null;
        }
    }
}