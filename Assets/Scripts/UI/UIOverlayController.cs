using UnityEngine;

namespace UI
{
    public class UIOverlayController : MonoBehaviour
    {
        private static readonly int HasLowHealth = Animator.StringToHash("HasLowHealth");
        private static readonly int Hit = Animator.StringToHash("Hit");
        private static readonly int OnDeath = Animator.StringToHash("OnDeath");
        public BoolChannelSO OnLowHealthChannel;
        public VoidChannelSO OnHitChannel;
        public BoolChannelSO OnDeathChannel;
        [SerializeField]private Animator overlayAnimator;

        private void OnEnable()
        {
            OnLowHealthChannel.Subscribe(SetLowHealth);
            OnHitChannel.Subscribe(SetHit);
            OnDeathChannel.Subscribe(SetDead);
        }private void OnDisable()
        {
            OnLowHealthChannel.Unsubscribe(SetLowHealth);
            OnHitChannel.Unsubscribe(SetHit);
            OnDeathChannel.Unsubscribe(SetDead);
        }

        private void SetHit() => overlayAnimator.SetTrigger(Hit);

        private void SetLowHealth(bool obj) => overlayAnimator.SetBool(HasLowHealth, obj);

        private void SetDead(bool obj)
        {
            if (!obj)
            {
                return;
            }
            overlayAnimator.SetTrigger(OnDeath);
        }
    }
}