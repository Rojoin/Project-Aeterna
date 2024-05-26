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
                Rotation(moveDir);

                _characterController.Move(moveDir * (time * player.speed));
                //transform.position += moveDir * (time * speed);


                _playerAnimatorController.SetFloat("Blend", dir.magnitude);

                yield return null;
            }

            _playerAnimatorController.SetFloat("Blend", 0);
        }

        /// <summary>
        /// Checks if the player is able to move towards the input direction.
        /// Returns true if the player can move at least in one direction. 
        /// </summary>
        /// <param name="dir">InputDirection.</param>
        /// <param name="time">Delta Time.</param>
        /// <param name="moveDir">Direction to be return. It changes depending on the available direction.</param>
        /// <returns></returns>
        private bool CanMove(Vector2 dir, float time, ref Vector3 moveDir)
        {
            moveDir = new Vector3(dir.x, 0, dir.y);
            bool canMove = !IsColliding(moveDir, time);

            if (!canMove)
            {
                Vector3 moveDirX = new Vector3(dir.x, 0, 0);
                canMove = !IsColliding(moveDirX, time);
                if (canMove)
                {
                    moveDir = moveDirX;
                }
                else
                {
                    Vector3 moveDirY = new Vector3(0, 0, dir.y);
                    canMove = !IsColliding(moveDirY, time);
                    if (canMove)
                    {
                        moveDir = moveDirY;
                    }
                }
            }

            return canMove;
        }

        /// <summary>
        /// Returns true if the player is collinding.
        /// </summary>
        /// <param name="moveDir">Direction of the player</param>
        /// <param name="time">Delta Time</param>
        /// <returns></returns>
        private bool IsColliding(Vector3 moveDir, float time)
        {
            var position = transform.position;
            return Physics.CapsuleCast(position, position + Vector3.up * _characterController.height,
                _characterController.radius, moveDir, player.speed * time);
        }

        private void Rotation(Vector3 moveDir)
        {
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);
        }
        public void Rotate(Vector3 newDirection)
        {
            transform.forward = Vector3.Slerp(transform.forward, newDirection, 1);
        }
    }
}