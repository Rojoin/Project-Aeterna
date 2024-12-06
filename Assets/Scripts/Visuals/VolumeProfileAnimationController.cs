using System;
using UnityEngine;

namespace Visuals
{
    public class VolumeProfileAnimationController : MonoBehaviour
    {
        private static readonly int OnChamberComplete = Animator.StringToHash("OnChamberComplete");
        private static readonly int OnCardSpawn = Animator.StringToHash("OnCardSpawn");
        private static readonly int OnPlayerDeath = Animator.StringToHash("OnPlayerDeath");
        public VoidChannelSO onCardSpawn;
        public VoidChannelSO onChamberComplete;
        public BoolChannelSO onDeath;
        private Animator animator;


        private void OnEnable()
        {
            onCardSpawn.Subscribe(ActivateSpawnAnimation);
            onDeath.Subscribe(ActivateDeathAnimation);
            onChamberComplete.Subscribe(ActivateChamberCompleteAnimation);
            animator = GetComponent<Animator>();
        }

        private void OnDisable()
        {
            onCardSpawn.Unsubscribe(ActivateSpawnAnimation);
            onDeath.Unsubscribe(ActivateDeathAnimation);
            onChamberComplete.Unsubscribe(ActivateChamberCompleteAnimation);
        }
        
        private void ActivateDeathAnimation(bool obj)
        {
            animator.SetTrigger(OnPlayerDeath);
        }

        private void ActivateChamberCompleteAnimation()
        {
            animator.SetTrigger(OnChamberComplete);
        }

        private void ActivateSpawnAnimation()
        {
            animator.SetTrigger(OnCardSpawn);
        }
    }
}