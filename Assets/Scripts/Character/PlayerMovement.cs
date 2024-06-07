using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Character
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Animator _playerAnimatorController;
        [SerializeField] private Vector2ChannelSO OnMoveChannel;
        [SerializeField] public UnityEvent<Vector2> OnMovement;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private EntitySO player;

        private CharacterController _characterController;
        private Coroutine movement;
        private const float angle = -45;
        private Vector3 rotatedMoveDir;

        public float maxSpeed;

        private void OnEnable()
        {
            _characterController = GetComponent<CharacterController>();
            OnMoveChannel.Subscribe(Move);
        }

        private void OnDisable()
        {
            OnMoveChannel.Unsubscribe(Move);
            if (movement != null)
            {
                StopCoroutine(movement);
            }
        }

        private void Move(Vector2 dir)
        {
            OnMovement.Invoke(dir);
            if (movement != null)
            {
                StopCoroutine(movement);
            }

            movement = StartCoroutine(Movement(dir));
        }

        /// <summary>
        /// Movement Corroutine
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private IEnumerator Movement(Vector2 dir)
        {
            while (dir != Vector2.zero)
            {
                Vector3 moveDir = new Vector3(dir.x, 0, dir.y);
                float time = Time.deltaTime;
                rotatedMoveDir = Quaternion.AngleAxis(angle, Vector3.up) * moveDir;
                Rotation(rotatedMoveDir);

                _characterController.Move(rotatedMoveDir * (time * player.speed));
                //transform.position += moveDir * (time * speed);


                _playerAnimatorController.SetFloat("Blend", dir.magnitude);

                yield return null;
            }

            _playerAnimatorController.SetFloat("Blend", 0);
            rotatedMoveDir = Vector3.zero;
        }
        
        
        private void Rotation(Vector3 moveDir)
        {
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);
        }

        public void Rotate(Vector3 newDirection)
        {
            transform.forward = Vector3.Slerp(transform.forward, newDirection, 1);
        }

        public Vector3 GetRotatedMoveDir() => rotatedMoveDir;
    }
}